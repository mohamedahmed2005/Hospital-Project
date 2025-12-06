using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Hospital.BBL.Services.Interfaces;
using Hospital.BBL.DTOs.AppointmentDTOs;
using Hospital.DAL.Models.AppointmentModule;
using Hospital.BBL.DTOs.DoctorDTOs;
using Hospital.BBL.DTOs.DepartmentDTOs;
using Hospital.DAL.Models.Shared;
using System.Text.RegularExpressions;

namespace Hospital.PL.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ChatController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ChatController> _logger;
        private readonly IDoctorService _doctorService;
        private readonly IAppointmentService _appointmentService;
        private readonly IDepartmentService _departmentService;
        private readonly IPatientService _patientService;
        private readonly UserManager<ApplicationUser> _userManager;

        // New Gemini model
        private const string GeminiModel = "gemini-2.0-flash";

        private static string GetGeminiUrl() =>
            $"https://generativelanguage.googleapis.com/v1beta/models/{GeminiModel}:generateContent";

        public ChatController(
            IHttpClientFactory httpClientFactory,
            ILogger<ChatController> logger,
            IDoctorService doctorService,
            IAppointmentService appointmentService,
            IDepartmentService departmentService,
            IPatientService patientService,
            UserManager<ApplicationUser> userManager)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _doctorService = doctorService;
            _appointmentService = appointmentService;
            _departmentService = departmentService;
            _patientService = patientService;
            _userManager = userManager;
        }

        public sealed class ChatRequest
        {
            public string? Message { get; set; }
        }

        public sealed class ChatResponse
        {
            public string Reply { get; set; } = string.Empty;
            public string? Error { get; set; }
        }

        private string GetDoctorsSummary(string? specializationFilter = null, string? departmentFilter = null)
        { 
            var doctorsQuery = _doctorService.GetAllDoctors(false);

            // Filter by specialization/category
            if (!string.IsNullOrWhiteSpace(specializationFilter))
            {
                doctorsQuery = doctorsQuery
                    .Where(d => !string.IsNullOrWhiteSpace(d.Specialization) &&
                                d.Specialization.Contains(specializationFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by department
            if (!string.IsNullOrWhiteSpace(departmentFilter))
            {
                var allDepartments = _departmentService.GetAllDepartments(false).ToList();
                var matchingDepartment = allDepartments.FirstOrDefault(d => 
                    d.Name.Contains(departmentFilter, StringComparison.OrdinalIgnoreCase) ||
                    d.DepartmentSpeciality.Contains(departmentFilter, StringComparison.OrdinalIgnoreCase));

                if (matchingDepartment != null)
                {
                    doctorsQuery = doctorsQuery.Where(d => d.DepartmentId == matchingDepartment.Id);
                }
            }

            var doctors = doctorsQuery.Take(10).ToList();
            if (doctors.Count == 0)
            {
                if (!string.IsNullOrWhiteSpace(specializationFilter) && !string.IsNullOrWhiteSpace(departmentFilter))
                {
                    return $"There are currently no doctors with specialization \"{specializationFilter}\" in the \"{departmentFilter}\" department.";
                }
                if (!string.IsNullOrWhiteSpace(specializationFilter))
                {
                    return $"There are currently no doctors with a specialization matching \"{specializationFilter}\" in the system.";
                }
                if (!string.IsNullOrWhiteSpace(departmentFilter))
                {
                    return $"There are currently no doctors in the \"{departmentFilter}\" department.";
                }

                return "There are currently no doctors in the system.";
            }

            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(specializationFilter) && !string.IsNullOrWhiteSpace(departmentFilter))
            {
                sb.AppendLine($"Here are the doctors with specialization \"{specializationFilter}\" in the \"{departmentFilter}\" department:");
            }
            else if (!string.IsNullOrWhiteSpace(specializationFilter))
            {
                sb.AppendLine($"Here are the doctors with specialization \"{specializationFilter}\" at NovaHealth Hospital:");
            }
            else if (!string.IsNullOrWhiteSpace(departmentFilter))
            {
                sb.AppendLine($"Here are the doctors in the \"{departmentFilter}\" department:");
            }
            else
            {
                sb.AppendLine("Here are the doctors at NovaHealth Hospital:");
            }

            foreach (var d in doctors)
            {
                var departmentInfo = !string.IsNullOrWhiteSpace(d.Department) ? $" - Department: {d.Department}" : "";
                sb.AppendLine($"* **{d.Name}** - {d.Specialization}{departmentInfo}");
            }
            
            if (doctors.Count >= 10)
            {
                sb.AppendLine($"\n(Showing first 10 doctors. There may be more available.)");
            }
            
            sb.AppendLine("\nLet me know if you need any more information about a specific doctor!");
            return sb.ToString();
        }

        private (string? specialization, string? department) ExtractDoctorFilters(string message)
        {
            var lower = message.ToLowerInvariant();
            string? specialization = null;
            string? department = null;

            // Common specialization keywords
            var specializationKeywords = new[] { "specialization", "speciality", "category", "specialize", "expertise" };
            var departmentKeywords = new[] { "department", "dept", "division", "unit" };

            // Try to extract specialization
            foreach (var keyword in specializationKeywords)
            {
                var index = lower.IndexOf(keyword);
                if (index >= 0)
                {
                    var afterKeyword = message.Substring(index + keyword.Length).Trim();
                    // Look for "is", "=", or direct value
                    var parts = afterKeyword.Split(new[] { " is ", "=", ":", ",", ".", " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0)
                    {
                        specialization = parts[0].Trim();
                        // Remove common words
                        specialization = specialization.Replace("that", "").Replace("its", "").Replace("the", "").Trim();
                        if (specialization.Length > 2)
                            break;
                    }
                }
            }

            // Try to extract department
            foreach (var keyword in departmentKeywords)
            {
                var index = lower.IndexOf(keyword);
                if (index >= 0)
                {
                    var afterKeyword = message.Substring(index + keyword.Length).Trim();
                    var parts = afterKeyword.Split(new[] { " is ", "=", ":", ",", ".", " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0)
                    {
                        department = parts[0].Trim();
                        department = department.Replace("that", "").Replace("its", "").Replace("the", "").Trim();
                        if (department.Length > 2)
                            break;
                    }
                }
            }

            // If no explicit keyword found, try to detect common specializations/departments
            if (string.IsNullOrWhiteSpace(specialization) && string.IsNullOrWhiteSpace(department))
            {
                var commonSpecializations = new[] { "heart", "cardiology", "pediatrics", "orthopedics", "neurology", "surgery", "dermatology", "psychiatry" };
                var commonDepartments = new[] { "cardiology", "pediatrics", "orthopedics", "neurology", "surgery", "emergency", "radiology" };

                foreach (var spec in commonSpecializations)
                {
                    if (lower.Contains(spec))
                    {
                        specialization = spec;
                        break;
                    }
                }

                foreach (var dept in commonDepartments)
                {
                    if (lower.Contains(dept) && !lower.Contains("specialization"))
                    {
                        department = dept;
                        break;
                    }
                }
            }

            return (specialization, department);
        }

        private async Task<ChatResponse> TryHandleAppointmentCommandAsync(string message)
        {
            // Expected simple format, for example:
            // add appointment doctorId=2 date=2025-12-20 time=14:00 type=Consultation notes=Checkup
            // Note: patientId is automatically taken from the logged-in user
            var lower = message.ToLowerInvariant();
            if (!lower.StartsWith("add appointment") && !lower.StartsWith("book appointment") && 
                !lower.StartsWith("create appointment") && !lower.StartsWith("schedule appointment"))
            {
                return new ChatResponse { Error = null, Reply = string.Empty };
            }

            try
            {
                // Check if user is authenticated
                if (!User.Identity?.IsAuthenticated ?? true)
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = "You must be logged in to create an appointment. Please log in first."
                    };
                }

                // Get current user and find their patient profile
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = "Unable to identify your user account. Please log in again."
                    };
                }

                var patients = _patientService.GetAllPatients(false);
                var patient = patients.FirstOrDefault(p => p.Email == user.Email);
                
                if (patient == null)
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = "Patient profile not found. Please create a patient profile first before booking an appointment."
                    };
                }

                var patientId = patient.Id;

                var parts = message.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                   .Skip(2); // skip "add" "appointment" / "book" "appointment"

                // Extract key=value pairs with support for quotes and spaces
                var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                var matches = Regex.Matches(message, @"(\w+)=(""[^""]*""|\S+)");
                foreach (Match m in matches)
                {
                    var key = m.Groups[1].Value;
                    var value = m.Groups[2].Value;

                    // Remove wrapping quotes if present
                    if (value.StartsWith("\"") && value.EndsWith("\""))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    dict[key] = value;
                }


                // patientId is no longer required - we use the logged-in user's ID
                if (!dict.TryGetValue("doctorId", out var doctorIdStr) ||
                    !dict.TryGetValue("date", out var dateStr) ||
                    !dict.TryGetValue("time", out var timeStr))
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = "To add an appointment via chat, use: add appointment doctorId=2 date=2025-12-20 time=14:00 type=Consultation notes=YourNotes\n" +
                               "Note: Your patient ID will be automatically used from your account."
                    };
                }

                // Validate and parse doctor ID
                if (!int.TryParse(doctorIdStr, out int doctorId) || doctorId <= 0)
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = "Invalid doctor ID. Please provide a valid positive number."
                    };
                }

                // Validate doctor exists
                var doctor = _doctorService.GetDoctorById(doctorId);
                if (doctor == null)
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = $"Doctor with ID {doctorId} does not exist. Please check the doctor ID and try again."
                    };
                }

                // Validate and parse date
                if (!DateOnly.TryParse(dateStr, out var date))
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = "Invalid date format. Please use format: YYYY-MM-DD (e.g., 2025-12-20)"
                    };
                }

                // Check if date is in the past
                if (date < DateOnly.FromDateTime(DateTime.Now))
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = "Cannot create an appointment for a past date. Please select a future date."
                    };
                }

                // Validate and parse time
                if (!TimeSpan.TryParse(timeStr, out var time))
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = "Invalid time format. Please use format: HH:mm (e.g., 14:00 or 2:00 PM)"
                    };
                }

                // Check for appointment conflicts (same doctor, same date and time)
                var existingAppointments = _appointmentService.GetAppointmentsByDoctorAndDate(doctorId, date);
                var conflictingAppointment = existingAppointments.FirstOrDefault(a => 
                    a.Appointment_Time == time && a.Status != AppointmentStatus.cancelled);

                if (conflictingAppointment != null)
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = $"Doctor #{doctorId} already has an appointment scheduled on {date:yyyy-MM-dd} at {time:hh\\:mm}. Please choose a different time."
                    };
                }

                AppointmentType appointmentType = AppointmentType.Consultation;
                if (dict.TryGetValue("type", out var typeStr))
                {
                    if (!Enum.TryParse<AppointmentType>(typeStr, ignoreCase: true, out appointmentType))
                    {
                        appointmentType = AppointmentType.Consultation;
                    }
                }

                var notes = dict.TryGetValue("notes", out var notesStr)
                    ? notesStr.Replace('_', ' ')
                    : "Created via chat assistant.";

                // Validate notes length
                if (notes.Length < 3)
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = "Notes must be at least 3 characters long."
                    };
                }

                if (notes.Length > 1000)
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = "Notes cannot exceed 1000 characters."
                    };
                }

                var dto = new AddAppointmentDto
                {
                    Appointment_Date = date,
                    Appointment_Time = time,
                    AppointmentType = appointmentType,
                    Status = AppointmentStatus.Scheduled,
                    IsAvailable = true,
                    Notes = notes,
                    PatientId = patientId,
                    DoctorId = doctorId
                };

                var addedId = _appointmentService.AddAppointment(dto);
                if (addedId <= 0)
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = "I tried to create the appointment, but the backend reported a failure. Please check all the information and try again."
                    };
                }

                return new ChatResponse
                {
                    Error = null,
                    Reply = $"✅ Appointment created successfully!\n" +
                           $"Patient: {patient.Name} (ID: {patientId})\n" +
                           $"Doctor: {doctor.Name} (ID: {doctorId})\n" +
                           $"Date: {date:yyyy-MM-dd}\n" +
                           $"Time: {time:hh\\:mm}\n" +
                           $"Type: {appointmentType}\n" +
                           $"Appointment ID: {addedId}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating appointment from chat command.");
                return new ChatResponse
                {
                    Error = null,
                    Reply = $"I encountered an error while processing your appointment request: {ex.Message}. Please check the format and try again."
                };
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
                return BadRequest(new ChatResponse { Error = "Message is required." });

            var lower = request.Message.ToLowerInvariant();

            // 1) Special command: list doctors from backend
            // Check for doctor-related queries
            if (lower.Contains("doctor") || lower.Contains("doctors"))
            {
                // Check if it's a query about doctors (list, show, find, etc.)
                var isDoctorQuery = lower.Contains("list") || lower.Contains("show") || 
                                   lower.Contains("find") || lower.Contains("get") ||
                                   lower.Contains("display") || lower.Contains("see");

                if (isDoctorQuery)
                {
                    var (specialization, department) = ExtractDoctorFilters(request.Message);
                    var doctorsText = GetDoctorsSummary(specialization, department);
                    return Ok(new ChatResponse { Reply = doctorsText });
                }
            }

            // 2) Special command: add appointment via backend
            var appointmentResult = await TryHandleAppointmentCommandAsync(request.Message);
            if (!string.IsNullOrWhiteSpace(appointmentResult.Reply) && string.IsNullOrEmpty(appointmentResult.Error))
            {
                return Ok(appointmentResult);
            }

            // Read Gemini API key from environment variable
            var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogError("GEMINI_API_KEY environment variable is not set.");
                return StatusCode(500, new ChatResponse { Error = "AI configuration is missing on the server." });
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                // Build Gemini request body with enhanced system instruction
                var systemInstruction =
                    "You are Nova, a helpful AI assistant for NovaHealth Hospital. " +
                    "Your capabilities include:\n" +
                    "1. **Doctor Information**: You can help users find doctors by:\n" +
                    "   - Listing all doctors\n" +
                    "   - Finding doctors by specialization/category (e.g., cardiology, pediatrics, heart, surgery)\n" +
                    "   - Finding doctors by department (e.g., Cardiology Department, Emergency Department)\n" +
                    "   - When users ask about doctors, guide them to use queries like 'list all doctors', 'show doctors with specialization [name]', or 'find doctors in [department]'\n\n" +
                    "2. **Appointments**: You can help users create appointments. To create an appointment, users should use the format:\n" +
                    "   'add appointment doctorId=[ID] date=YYYY-MM-DD time=HH:mm type=[Type] notes=[Notes]'\n" +
                    "   - Valid appointment types: Consultation, FollowUp, Emergency, Surgery, Checkup\n" +
                    "   - Date must be in the future\n" +
                    "   - Time format: HH:mm (24-hour) or use standard time formats\n" +
                    "   - Patient ID is automatically taken from the logged-in user's account (no need to specify patientId)\n" +
                    "   - The system will validate that the patient and doctor exist, check for conflicts, and ensure data validity\n\n" +
                    "3. **General Information**: Provide information about hospital services, departments, and general medical information.\n\n" +
                    "**Important Rules**:\n" +
                    "- You must NOT give medical diagnoses or prescribe treatments\n" +
                    "- Keep answers clear, friendly, and concise\n" +
                    "- If a user asks about doctors or appointments in natural language, guide them on how to use the system\n" +
                    "- Always be helpful and professional";

                var body = new
                {
                    contents = new[]
                    {
                        new
                            {
                                parts = new[]
                                {
                                    new { text = systemInstruction + "\n\nUser question: " + request.Message }
                                }
                            }
                    }
                };

                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Add API key in header
                client.DefaultRequestHeaders.Add("X-Goog-Api-Key", apiKey);

                var url = GetGeminiUrl();
                var response = await client.PostAsync(url, content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Gemini API error: {Status} - {Body}", response.StatusCode, responseText);
                    return StatusCode(500, new ChatResponse { Error = "AI service error. Please try again later." });
                }

                using var doc = JsonDocument.Parse(responseText);
                var root = doc.RootElement;

                // Extract first candidate text
                if (!root.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
                {
                    _logger.LogError("Gemini response missing candidates: {Body}", responseText);
                    return StatusCode(500, new ChatResponse { Error = "AI response was empty." });
                }

                var contentElement = candidates[0].GetProperty("content");
                if (!contentElement.TryGetProperty("parts", out var parts))
                {
                    _logger.LogError("Gemini response missing parts: {Body}", responseText);
                    return StatusCode(500, new ChatResponse { Error = "AI response was incomplete." });
                }

                var sb = new StringBuilder();
                foreach (var part in parts.EnumerateArray())
                {
                    if (part.TryGetProperty("text", out var textElement))
                        sb.Append(textElement.GetString());
                }

                var reply = sb.ToString();
                if (string.IsNullOrWhiteSpace(reply))
                    reply = "I couldn't generate an answer right now, please try another question.";

                return Ok(new ChatResponse { Reply = reply });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling Google Gemini API.");
                return StatusCode(500, new ChatResponse { Error = "Unexpected error talking to AI." });
            }
        }
    }
}
