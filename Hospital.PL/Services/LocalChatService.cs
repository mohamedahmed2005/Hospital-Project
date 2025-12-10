using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Hospital.BBL.Services.Interfaces;
using Hospital.BBL.DTOs.DepartmentDTOs;
using Hospital.BBL.DTOs.DoctorDTOs;

namespace Hospital.PL.Services
{
    public class LocalChatService
    {
        private readonly IDoctorService _doctorService;
        private readonly IDepartmentService _departmentService;

        // Enhanced static data for more comprehensive responses
        private readonly Dictionary<string, List<string>> _commonSymptoms;
        private readonly Dictionary<string, string> _commonTreatments;
        private readonly Dictionary<string, List<string>> _preventiveCare;
        private readonly Dictionary<string, string> _healthTips;
        private readonly Dictionary<string, List<string>> _medicationInfo;
        private readonly Dictionary<string, string> _diagnosticInfo;

        public LocalChatService(
            IDoctorService doctorService,
            IDepartmentService departmentService)
        {
            _doctorService = doctorService;
            _departmentService = departmentService;

            // Initialize enhanced static data
            _commonSymptoms = InitializeCommonSymptoms();
            _commonTreatments = InitializeCommonTreatments();
            _preventiveCare = InitializePreventiveCare();
            _healthTips = InitializeHealthTips();
            _medicationInfo = InitializeMedicationInfo();
            _diagnosticInfo = InitializeDiagnosticInfo();
        }

        #region Static Data Initialization

        private Dictionary<string, List<string>> InitializeCommonSymptoms()
        {
            return new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Fever"] = new List<string> { "High temperature", "Chills", "Sweating", "Headache", "Muscle aches" },
                ["Cold"] = new List<string> { "Runny nose", "Sneezing", "Sore throat", "Cough", "Mild fever" },
                ["Flu"] = new List<string> { "High fever", "Severe body aches", "Fatigue", "Dry cough", "Headache" },
                ["Allergies"] = new List<string> { "Sneezing", "Itchy eyes", "Runny nose", "Rash", "Watery eyes" },
                ["Migraine"] = new List<string> { "Severe headache", "Nausea", "Sensitivity to light", "Aura", "Vomiting" },
                ["Anxiety"] = new List<string> { "Racing heart", "Sweating", "Trembling", "Shortness of breath", "Worry" },
                ["Depression"] = new List<string> { "Persistent sadness", "Loss of interest", "Fatigue", "Sleep changes", "Appetite changes" },
                ["Arthritis"] = new List<string> { "Joint pain", "Stiffness", "Swelling", "Reduced mobility", "Warm joints" },
                ["Diabetes"] = new List<string> { "Increased thirst", "Frequent urination", "Fatigue", "Blurred vision", "Slow healing" },
                ["Hypertension"] = new List<string> { "Headaches", "Shortness of breath", "Nosebleeds", "Flushing", "Dizziness" }
            };
        }

        private Dictionary<string, string> InitializeCommonTreatments()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Cold"] = "Rest, fluids, over-the-counter cold medication, humidifier",
                ["Flu"] = "Antiviral medication, rest, fluids, fever reducers",
                ["Fever"] = "Fever reducers (acetaminophen/ibuprofen), cool compresses, hydration",
                ["Headache"] = "Pain relievers, rest in dark room, hydration, stress management",
                ["Allergies"] = "Antihistamines, nasal sprays, allergy shots, allergen avoidance",
                ["Anxiety"] = "Therapy, medication (SSRIs), relaxation techniques, lifestyle changes",
                ["Depression"] = "Therapy, medication (antidepressants), exercise, social support",
                ["Arthritis"] = "Pain relievers, anti-inflammatory drugs, physical therapy, joint protection",
                ["Diabetes"] = "Blood sugar monitoring, medication/insulin, diet control, exercise",
                ["Hypertension"] = "Blood pressure medication, low-sodium diet, exercise, stress reduction"
            };
        }

        private Dictionary<string, List<string>> InitializePreventiveCare()
        {
            return new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["General"] = new List<string>
                {
                    "Annual physical exams",
                    "Regular blood pressure checks",
                    "Cholesterol screening",
                    "Blood sugar testing",
                    "Cancer screenings (age-appropriate)",
                    "Dental checkups every 6 months",
                    "Eye exams regularly",
                    "Immunizations up to date"
                },
                ["Women"] = new List<string>
                {
                    "Pap smear (starting at 21)",
                    "Mammogram (starting at 40-50)",
                    "Bone density test (post-menopause)",
                    "Breast self-exams monthly",
                    "HPV vaccination if eligible",
                    "Prenatal care if pregnant"
                },
                ["Men"] = new List<string>
                {
                    "Prostate screening (starting at 50)",
                    "Testicular self-exams",
                    "Colon cancer screening (starting at 45)",
                    "Heart health screening",
                    "PSA test if recommended"
                },
                ["Seniors"] = new List<string>
                {
                    "Annual Medicare wellness visit",
                    "Fall risk assessment",
                    "Medication review",
                    "Cognitive screening",
                    "Bone density testing",
                    "Pneumonia vaccine",
                    "Shingles vaccine",
                    "Flu vaccine annually"
                }
            };
        }

        private Dictionary<string, string> InitializeHealthTips()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Hydration"] = "Drink at least 8 glasses (64 oz) of water daily. Increase intake during exercise, hot weather, or illness.",
                ["Sleep"] = "Aim for 7-9 hours of quality sleep per night. Maintain consistent sleep schedule even on weekends.",
                ["Nutrition"] = "Eat balanced meals with fruits, vegetables, whole grains, lean proteins, and healthy fats. Limit processed foods.",
                ["Exercise"] = "150 minutes moderate exercise or 75 minutes vigorous exercise weekly. Include strength training 2x weekly.",
                ["Stress"] = "Practice stress-reduction techniques: meditation, deep breathing, yoga, hobbies, social connections.",
                ["Smoking"] = "Quit smoking to reduce risk of cancer, heart disease, stroke, and respiratory illnesses. Seek help if needed.",
                ["Alcohol"] = "Limit alcohol to 1 drink daily for women, 2 for men. Avoid binge drinking.",
                ["Screenings"] = "Stay current with age-appropriate health screenings and vaccinations.",
                ["Sun Protection"] = "Use SPF 30+ sunscreen, wear protective clothing, avoid peak sun hours (10 AM - 4 PM).",
                ["Mental Health"] = "Prioritize mental wellbeing. Seek help for persistent sadness, anxiety, or mood changes."
            };
        }

        private Dictionary<string, List<string>> InitializeMedicationInfo()
        {
            return new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Pain Relievers"] = new List<string>
                {
                    "Acetaminophen (Tylenol) - For pain and fever",
                    "Ibuprofen (Advil) - For pain, inflammation, fever",
                    "Naproxen (Aleve) - For pain and inflammation",
                    "Aspirin - For pain, fever, anti-inflammatory",
                    "Never exceed recommended doses"
                },
                ["Cold & Flu"] = new List<string>
                {
                    "Decongestants (Pseudoephedrine) - For nasal congestion",
                    "Antihistamines (Diphenhydramine) - For runny nose, sneezing",
                    "Expectorants (Guaifenesin) - To loosen mucus",
                    "Cough suppressants (Dextromethorphan) - For dry cough",
                    "Check for drug interactions"
                },
                ["Allergies"] = new List<string>
                {
                    "Loratadine (Claritin) - Non-drowsy antihistamine",
                    "Cetirizine (Zyrtec) - Non-drowsy antihistamine",
                    "Fexofenadine (Allegra) - Non-drowsy antihistamine",
                    "Nasal sprays (Fluticasone) - For nasal symptoms",
                    "Eye drops (Ketotifen) - For eye allergies"
                },
                ["Antibiotics"] = new List<string>
                {
                    "Amoxicillin - Common for bacterial infections",
                    "Azithromycin - For respiratory infections",
                    "Ciprofloxacin - For UTIs, infections",
                    "Complete full course as prescribed",
                    "Never share antibiotics"
                }
            };
        }

        private Dictionary<string, string> InitializeDiagnosticInfo()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Blood Test"] = "Common tests: CBC (blood count), Chemistry panel (electrolytes), Lipid panel (cholesterol), Blood sugar. Fasting may be required.",
                ["Urine Test"] = "Tests for: Infection, kidney function, diabetes, pregnancy. Clean catch sample needed.",
                ["X-Ray"] = "Uses radiation to image bones, chest, abdomen. Quick and painless. Inform if pregnant.",
                ["MRI"] = "Uses magnetic fields for detailed images of organs, tissues. No radiation. May take 30-60 minutes.",
                ["CT Scan"] = "Combines X-rays for cross-sectional images. May use contrast dye. Quick procedure.",
                ["Ultrasound"] = "Uses sound waves for images. Safe, no radiation. Used for pregnancy, abdominal organs.",
                ["EKG"] = "Records heart's electrical activity. Painless, takes 5-10 minutes. Checks heart rhythm.",
                ["Stress Test"] = "Monitors heart during exercise. Assesses heart function and blood flow.",
                ["Endoscopy"] = "Camera examination of digestive tract. May require sedation.",
                ["Biopsy"] = "Tissue sample for cancer diagnosis. Various methods (needle, surgical)."
            };
        }

        #endregion

        public string GenerateResponse(string userMessage)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                return GetWelcomeMessage();
            }

            var lower = userMessage.ToLowerInvariant().Trim();
            var originalMessage = userMessage.Trim();

            // Handle greetings
            if (IsGreeting(lower))
            {
                return HandleGreeting();
            }

            // Handle specific medical information queries
            if (IsMedicalInfoQuery(lower))
            {
                return HandleMedicalInfoQuery(originalMessage, lower);
            }

            // Handle doctor-related queries
            if (IsDoctorQuery(lower))
            {
                return HandleDoctorQuery(originalMessage, lower);
            }

            // Handle department queries
            if (IsDepartmentQuery(lower))
            {
                return HandleDepartmentQuery(originalMessage, lower);
            }

            // Handle appointment-related queries
            if (IsAppointmentQuery(lower))
            {
                return HandleAppointmentQuery(originalMessage, lower);
            }

            // Handle service queries
            if (IsServiceQuery(lower))
            {
                return HandleServiceQuery();
            }

            // Handle hours/contact queries
            if (IsContactQuery(lower))
            {
                return HandleContactQuery(lower);
            }

            // Handle emergency queries
            if (IsEmergencyQuery(lower))
            {
                return HandleEmergencyQuery();
            }

            // Handle general hospital information
            if (IsGeneralInfoQuery(lower))
            {
                return HandleGeneralInfoQuery();
            }

            // Handle help queries
            if (IsHelpQuery(lower))
            {
                return HandleHelpQuery();
            }

            // Handle thank you and goodbye
            if (IsThankYouOrGoodbye(lower))
            {
                return HandleThankYouOrGoodbye();
            }

            // Handle preventive care queries
            if (IsPreventiveCareQuery(lower))
            {
                return HandlePreventiveCareQuery(lower);
            }

            // Handle medication queries
            if (IsMedicationQuery(lower))
            {
                return HandleMedicationQuery(lower);
            }

            // Handle diagnostic test queries
            if (IsDiagnosticQuery(lower))
            {
                return HandleDiagnosticQuery(lower);
            }

            // Handle health tip queries
            if (IsHealthTipQuery(lower))
            {
                return HandleHealthTipQuery(lower);
            }

            // Handle specific department name queries
            var specificDept = TryFindSpecificDepartment(lower);
            if (specificDept != null)
            {
                return HandleSpecificDepartmentQuery(specificDept);
            }

            // Handle specific doctor name queries
            var specificDoctor = TryFindSpecificDoctor(lower);
            if (specificDoctor != null)
            {
                return HandleSpecificDoctorQuery(specificDoctor);
            }

            // Default response with intelligent suggestions
            return HandleDefaultResponse(originalMessage, lower);
        }

        #region Query Detection Methods

        private bool IsGreeting(string message)
        {
            var greetings = new[]
            {
                "hello", "hi", "hey", "greetings", "good morning", "good afternoon",
                "good evening", "howdy", "hi there", "hey there", "greeting",
                "what's up", "whats up", "sup", "morning", "afternoon", "evening",
                "hiya", "how are you", "how do you do"
            };
            return greetings.Any(g => message.Contains(g)) ||
                   message.StartsWith("hi ") || message.StartsWith("hey ") ||
                   message == "hi" || message == "hey" || message == "hello";
        }

        private bool IsMedicalInfoQuery(string message)
        {
            var medicalKeywords = new[]
            {
                "symptom", "symptoms", "condition", "disease", "illness", "sick",
                "feel sick", "not feeling well", "what's wrong", "whats wrong",
                "diagnose", "diagnosis", "what could it be", "might have"
            };
            return medicalKeywords.Any(k => message.Contains(k)) ||
                   _commonSymptoms.Keys.Any(s => message.Contains(s.ToLower()));
        }

        private bool IsDoctorQuery(string message)
        {
            var doctorKeywords = new[]
            {
                "doctor", "doctors", "physician", "physicians", "specialist", "specialists",
                "cardiologist", "pediatrician", "surgeon", "neurologist", "dermatologist",
                "psychiatrist", "orthopedic", "doctor's", "doctors'", "find doctor",
                "search doctor", "list doctor", "show doctor", "available doctor"
            };
            return doctorKeywords.Any(k => message.Contains(k));
        }

        private bool IsDepartmentQuery(string message)
        {
            var deptKeywords = new[]
            {
                "department", "departments", "dept", "division", "unit", "section",
                "clinic", "clinics", "ward", "wards", "department's", "departments'"
            };
            return deptKeywords.Any(k => message.Contains(k));
        }

        private bool IsAppointmentQuery(string message)
        {
            var appointmentKeywords = new[]
            {
                "appointment", "appointments", "book", "booking", "schedule", "scheduling",
                "visit", "visits", "consultation", "consultations", "meeting", "meetings",
                "reserve", "reservation", "reservations", "slot", "slots", "time slot"
            };
            return appointmentKeywords.Any(k => message.Contains(k));
        }

        private bool IsServiceQuery(string message)
        {
            var serviceKeywords = new[]
            {
                "service", "services", "offer", "offers", "provide", "provides",
                "treatment", "treatments", "care", "medical care", "healthcare",
                "what do you offer", "what services", "available services"
            };
            return serviceKeywords.Any(k => message.Contains(k));
        }

        private bool IsContactQuery(string message)
        {
            var contactKeywords = new[]
            {
                "hour", "hours", "open", "opening", "close", "closing", "time", "times",
                "when", "contact", "phone", "telephone", "email", "address", "location",
                "where", "call", "reach", "reachable", "available", "availability"
            };
            return contactKeywords.Any(k => message.Contains(k));
        }

        private bool IsEmergencyQuery(string message)
        {
            var emergencyKeywords = new[]
            {
                "emergency", "urgent", "emergency room", "er", "ambulance", "critical",
                "life threatening", "immediate", "asap", "as soon as possible", "911"
            };
            return emergencyKeywords.Any(k => message.Contains(k));
        }

        private bool IsGeneralInfoQuery(string message)
        {
            var infoKeywords = new[]
            {
                "about", "information", "info", "hospital", "what", "who", "tell me",
                "explain", "describe", "details", "more about", "learn", "know about"
            };
            return infoKeywords.Any(k => message.Contains(k));
        }

        private bool IsHelpQuery(string message)
        {
            var helpKeywords = new[]
            {
                "help", "how", "guide", "instructions", "assist", "assistance",
                "support", "what can you do", "what can you help", "capabilities",
                "features", "options", "commands"
            };
            return helpKeywords.Any(k => message.Contains(k));
        }

        private bool IsThankYouOrGoodbye(string message)
        {
            var phrases = new[]
            {
                "thank", "thanks", "thank you", "bye", "goodbye", "see you",
                "farewell", "appreciate", "grateful", "nice talking"
            };
            return phrases.Any(p => message.Contains(p));
        }

        private bool IsPreventiveCareQuery(string message)
        {
            var preventiveKeywords = new[]
            {
                "prevent", "prevention", "preventive", "checkup", "check-up",
                "screening", "screenings", "annual exam", "physical", "preventative",
                "early detection", "health screening"
            };
            return preventiveKeywords.Any(k => message.Contains(k));
        }

        private bool IsMedicationQuery(string message)
        {
            var medicationKeywords = new[]
            {
                "medication", "medicine", "pill", "drug", "prescription",
                "over the counter", "otc", "pharmacy", "dosage", "side effect",
                "take medicine", "how to take"
            };
            return medicationKeywords.Any(k => message.Contains(k));
        }

        private bool IsDiagnosticQuery(string message)
        {
            var diagnosticKeywords = new[]
            {
                "test", "tests", "testing", "diagnostic", "x-ray", "xray",
                "mri", "ct scan", "ultrasound", "blood test", "urine test",
                "ekg", "ecg", "scan", "imaging", "lab work", "biopsy"
            };
            return diagnosticKeywords.Any(k => message.Contains(k));
        }

        private bool IsHealthTipQuery(string message)
        {
            var healthTipKeywords = new[]
            {
                "tip", "tips", "advice", "recommendation", "suggestion",
                "how to stay healthy", "healthy living", "wellness",
                "lifestyle", "healthy habits", "good practice"
            };
            return healthTipKeywords.Any(k => message.Contains(k));
        }

        #endregion

        #region Query Handler Methods

        private string GetWelcomeMessage()
        {
            var doctors = _doctorService.GetAllDoctors(false).ToList();
            var departments = _departmentService.GetAllDepartments(false).ToList();

            return "Hello! I'm **Nova**, your intelligent healthcare assistant at NovaHealth Hospital. 👋\n\n" +
                   "I'm here to help you with:\n" +
                   "• **Medical Information** - Symptoms, conditions, treatments\n" +
                   "• **Finding Doctors** - Search by specialization, department, or name\n" +
                   "• **Department Information** - Learn about our specialized departments\n" +
                   "• **Booking Appointments** - Schedule your visit easily\n" +
                   "• **Hospital Services** - Discover what we offer\n" +
                   "• **Contact & Hours** - Get operating hours and contact information\n" +
                   "• **Preventive Care** - Screening and prevention information\n" +
                   "• **Medication Information** - Common medications and usage\n" +
                   "• **Diagnostic Tests** - Information about medical tests\n" +
                   "• **Health Tips** - Wellness and lifestyle advice\n\n" +
                   $"**Quick Stats:**\n" +
                   $"• {doctors.Count} qualified doctors available\n" +
                   $"• {departments.Count} specialized departments\n" +
                   $"• 24/7 Emergency Services\n" +
                   $"• Comprehensive medical services\n\n" +
                   "How can I assist you today?";
        }

        private string HandleGreeting()
        {
            var responses = new[]
            {
                "Hello! I'm Nova, your healthcare assistant. How can I help you today? 😊",
                "Hi there! Welcome to NovaHealth Hospital's virtual assistant. What can I do for you?",
                "Good day! I'm here to assist with medical information, appointments, and general inquiries.",
                "Welcome! I'm Nova, ready to help with your health questions and hospital services.",
                "Hello! How can I assist you with your healthcare needs today?"
            };
            return responses[new Random().Next(responses.Length)];
        }

        private string HandleMedicalInfoQuery(string originalMessage, string lower)
        {
            var sb = new StringBuilder();

            // Check for specific conditions mentioned
            var mentionedConditions = _commonSymptoms.Keys
                .Where(condition => lower.Contains(condition.ToLower()))
                .ToList();

            if (mentionedConditions.Any())
            {
                foreach (var condition in mentionedConditions.Take(2))
                {
                    sb.AppendLine($"**{condition} Information:**");

                    if (_commonSymptoms.ContainsKey(condition))
                    {
                        sb.AppendLine($"Common symptoms: {string.Join(", ", _commonSymptoms[condition])}");
                    }

                    if (_commonTreatments.ContainsKey(condition))
                    {
                        sb.AppendLine($"Common treatments: {_commonTreatments[condition]}");
                    }

                    sb.AppendLine();
                }

                sb.AppendLine("**When to see a doctor:**");
                sb.AppendLine("• Symptoms persist more than a few days");
                sb.AppendLine("• Symptoms are severe or worsening");
                sb.AppendLine("• You have underlying health conditions");
                sb.AppendLine("• You're experiencing emergency symptoms");
                sb.AppendLine();
                sb.AppendLine("**Disclaimer:** This is general information. Always consult a healthcare professional for diagnosis and treatment.");
            }
            else
            {
                sb.AppendLine("I understand you're asking about medical symptoms or conditions.");
                sb.AppendLine();
                sb.AppendLine("**Common conditions I can provide information about:**");

                foreach (var condition in _commonSymptoms.Keys.Take(6))
                {
                    sb.AppendLine($"• {condition}");
                }

                sb.AppendLine();
                sb.AppendLine("**Try asking about:**");
                sb.AppendLine("• Symptoms of [condition name]");
                sb.AppendLine("• Treatment for [condition name]");
                sb.AppendLine("• What causes [symptom]");
                sb.AppendLine();
                sb.AppendLine("**Or describe your symptoms and I'll try to help.**");
            }

            sb.AppendLine();
            sb.AppendLine("Would you like to book an appointment with a doctor? Ask: 'How do I book an appointment?'");

            return sb.ToString();
        }

        private string HandleDoctorQuery(string originalMessage, string lower)
        {
            var specialization = ExtractSpecialization(lower);
            var department = ExtractDepartmentName(lower);

            var doctors = _doctorService.GetAllDoctors(false).ToList();

            if (doctors.Count == 0)
            {
                return "I'm sorry, but there are currently no doctors registered in our system. " +
                       "Please check back later or contact our administration.";
            }

            var filteredDoctors = doctors.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(specialization))
            {
                filteredDoctors = filteredDoctors.Where(d =>
                    d.Specialization.Contains(specialization, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                var dept = _departmentService.GetAllDepartments(false)
                    .FirstOrDefault(d => d.Name.Contains(department, StringComparison.OrdinalIgnoreCase) ||
                                        d.DepartmentSpeciality.Contains(department, StringComparison.OrdinalIgnoreCase));

                if (dept != null)
                {
                    filteredDoctors = filteredDoctors.Where(d => d.DepartmentId == dept.Id);
                }
            }

            var doctorList = filteredDoctors.Take(15).ToList();

            if (doctorList.Count == 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine("I couldn't find any doctors matching your criteria.");

                if (!string.IsNullOrWhiteSpace(specialization))
                {
                    sb.AppendLine($"\nNo doctors found with specialization: **{specialization}**");
                }
                if (!string.IsNullOrWhiteSpace(department))
                {
                    sb.AppendLine($"\nNo doctors found in department: **{department}**");
                }

                sb.AppendLine("\n**Suggestions:**");
                sb.AppendLine("• Try asking: 'List all doctors'");
                sb.AppendLine("• Ask: 'Show departments' to see available departments");
                sb.AppendLine("• Try a different specialization or department name");

                return sb.ToString();
            }

            var response = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(specialization) && !string.IsNullOrWhiteSpace(department))
            {
                response.AppendLine($"**Doctors with specialization '{specialization}' in '{department}' department:**\n");
            }
            else if (!string.IsNullOrWhiteSpace(specialization))
            {
                response.AppendLine($"**Doctors specializing in '{specialization}':**\n");
            }
            else if (!string.IsNullOrWhiteSpace(department))
            {
                response.AppendLine($"**Doctors in '{department}' department:**\n");
            }
            else
            {
                response.AppendLine("**Available Doctors at NovaHealth Hospital:**\n");
            }

            foreach (var doctor in doctorList)
            {
                response.AppendLine($"**Dr. {doctor.Name}**");
                response.AppendLine($"• Specialization: {doctor.Specialization}");
                if (!string.IsNullOrWhiteSpace(doctor.Department))
                {
                    response.AppendLine($"• Department: {doctor.Department}");
                }
                response.AppendLine($"• Email: {doctor.Email}");
                response.AppendLine($"• Phone: {doctor.PhoneNumber}");
                response.AppendLine($"• ID: {doctor.Id} (use this ID to book appointments)");
                response.AppendLine();
            }

            if (doctorList.Count >= 15)
            {
                response.AppendLine($"(Showing first 15 doctors. There may be more available.)");
            }

            response.AppendLine("\n**To book an appointment with any doctor, use:**");
            response.AppendLine("`add appointment doctorId=[ID] date=YYYY-MM-DD time=HH:mm type=[Type] notes=[Notes]`");

            return response.ToString();
        }

        private string HandleDepartmentQuery(string originalMessage, string lower)
        {
            var departments = _departmentService.GetAllDepartments(false).ToList();

            if (departments.Count == 0)
            {
                return "Currently, there are no departments registered in the system. " +
                       "Please contact our administration for more information.";
            }

            var specificDept = TryFindSpecificDepartment(lower);
            if (specificDept != null)
            {
                return HandleSpecificDepartmentQuery(specificDept);
            }

            var sb = new StringBuilder();
            sb.AppendLine("**Our Departments:**\n");

            foreach (var dept in departments.Take(20))
            {
                sb.AppendLine($"**{dept.Name}**");
                if (!string.IsNullOrWhiteSpace(dept.DepartmentSpeciality))
                {
                    sb.AppendLine($"  • Speciality: {dept.DepartmentSpeciality}");
                }
                if (!string.IsNullOrWhiteSpace(dept.Location))
                {
                    sb.AppendLine($"  • Location: {dept.Location}");
                }
                if (!string.IsNullOrWhiteSpace(dept.KeyServices))
                {
                    sb.AppendLine($"  • Key Services: {dept.KeyServices}");
                }
                sb.AppendLine();
            }

            if (departments.Count > 20)
            {
                sb.AppendLine($"(Showing first 20 departments. There are {departments.Count} total departments.)");
            }

            sb.AppendLine("**Would you like to:**");
            sb.AppendLine("• Find doctors in a specific department? Just ask!");
            sb.AppendLine("• Learn more about a specific department? Mention the department name");
            sb.AppendLine("• Book an appointment? Use: 'add appointment doctorId=[ID] date=[DATE] time=[TIME] type=[TYPE] notes=[NOTES]'");

            return sb.ToString();
        }

        private string HandleAppointmentQuery(string originalMessage, string lower)
        {
            if (lower.Contains("how") || lower.Contains("book") || lower.Contains("create") ||
                lower.Contains("schedule") || lower.Contains("make") || lower.Contains("set up"))
            {
                return "**How to Book an Appointment:**\n\n" +
                       "To create an appointment, use this format:\n" +
                       "`add appointment doctorId=[ID] date=YYYY-MM-DD time=HH:mm type=[Type] notes=[Your Notes]`\n\n" +
                       "**Parameters:**\n" +
                       "• `doctorId`: The ID of the doctor (find this by asking 'list all doctors')\n" +
                       "• `date`: Appointment date in YYYY-MM-DD format (e.g., 2025-12-20)\n" +
                       "• `time`: Appointment time in HH:mm format (e.g., 14:00 or 2:00 PM)\n" +
                       "• `type`: Appointment type - Consultation, FollowUp, Emergency, Surgery, or Checkup\n" +
                       "• `notes`: Optional notes about your appointment\n\n" +
                       "**Example:**\n" +
                       "`add appointment doctorId=2 date=2025-12-20 time=14:00 type=Consultation notes=Regular checkup`\n\n" +
                       "**Important Notes:**\n" +
                       "• Your patient ID will be automatically used from your logged-in account\n" +
                       "• You must be logged in to create appointments\n" +
                       "• Dates must be in the future\n" +
                       "• Times should be during business hours (8 AM - 6 PM weekdays)\n\n" +
                       "**Would you like to:**\n" +
                       "• See available doctors? Ask: 'List all doctors'\n" +
                       "• Find doctors by specialization? Ask: 'Find doctors with specialization [name]'\n" +
                       "• Learn about appointment types? Just ask!";
            }

            if (lower.Contains("cancel") || lower.Contains("delete") || lower.Contains("remove"))
            {
                return "To cancel or modify an appointment, please:\n" +
                       "• Log in to your patient portal\n" +
                       "• Navigate to your appointments section\n" +
                       "• Select the appointment you want to cancel or modify\n\n" +
                       "For immediate assistance, please contact our appointment desk.";
            }

            return "I can help you book appointments! 📅\n\n" +
                   "**Quick Guide:**\n" +
                   "Use the format: `add appointment doctorId=[ID] date=YYYY-MM-DD time=HH:mm type=[Type] notes=[Notes]`\n\n" +
                   "**For detailed instructions, ask:** 'How do I book an appointment?'\n\n" +
                   "**To find doctors, ask:** 'List all doctors' or 'Find doctors with specialization [name]'";
        }

        private string HandleServiceQuery()
        {
            var departments = _departmentService.GetAllDepartments(false).ToList();
            var sb = new StringBuilder();

            sb.AppendLine("**Our Hospital Services:**\n");
            sb.AppendLine("NovaHealth Hospital offers comprehensive medical services across multiple specialized departments:\n");

            if (departments.Any())
            {
                foreach (var dept in departments.Take(15))
                {
                    sb.AppendLine($"**{dept.Name}**");
                    if (!string.IsNullOrWhiteSpace(dept.DepartmentSpeciality))
                    {
                        sb.AppendLine($"  Speciality: {dept.DepartmentSpeciality}");
                    }
                    if (!string.IsNullOrWhiteSpace(dept.KeyServices))
                    {
                        sb.AppendLine($"  Services: {dept.KeyServices}");
                    }
                    sb.AppendLine();
                }
            }
            else
            {
                sb.AppendLine("• **Emergency Medicine** - 24/7 emergency care\n");
                sb.AppendLine("• **Cardiology** - Heart and cardiovascular care\n");
                sb.AppendLine("• **Neurology** - Brain and nervous system care\n");
                sb.AppendLine("• **Orthopedics** - Bone and joint care\n");
                sb.AppendLine("• **Pediatrics** - Children's health care\n");
                sb.AppendLine("• **Oncology** - Cancer treatment and care\n");
                sb.AppendLine("• **Radiology** - Diagnostic imaging\n");
                sb.AppendLine("• **Laboratory Services** - Diagnostic testing\n");
                sb.AppendLine("• **Physical Therapy** - Rehabilitation services\n");
                sb.AppendLine("• **Mental Health** - Psychiatry and counseling\n");
            }

            sb.AppendLine("**Additional Services:**\n");
            sb.AppendLine("• **Telemedicine** - Virtual consultations\n");
            sb.AppendLine("• **Health Check-ups** - Comprehensive exams\n");
            sb.AppendLine("• **Vaccination Clinic** - Immunizations\n");
            sb.AppendLine("• **Pharmacy Services** - Medication dispensing\n");
            sb.AppendLine("• **Nutrition Counseling** - Dietary guidance\n");
            sb.AppendLine("• **Senior Care** - Elderly health services\n");

            sb.AppendLine("**Would you like to know more about:**");
            sb.AppendLine("• A specific department? Just mention the department name");
            sb.AppendLine("• Booking an appointment? Ask: 'How do I book an appointment?'");
            sb.AppendLine("• Finding a doctor? Ask: 'List all doctors'");

            return sb.ToString();
        }

        private string HandleContactQuery(string message)
        {
            var sb = new StringBuilder();
            sb.AppendLine("**Contact Information & Hours:**\n");

            if (message.Contains("hour") || message.Contains("open") || message.Contains("time") ||
                message.Contains("when") || message.Contains("available"))
            {
                sb.AppendLine("**Operating Hours:**\n");
                sb.AppendLine("• **Emergency Department**: Open 24/7, 365 days a year\n");
                sb.AppendLine("• **General Departments**: Monday - Friday: 8:00 AM - 6:00 PM\n");
                sb.AppendLine("• **Weekend Services**: Saturday - Sunday: 9:00 AM - 4:00 PM\n");
                sb.AppendLine("• **Holidays**: Emergency services only (24/7)\n");
                sb.AppendLine("• **Appointment Booking**: Available online 24/7\n");
                sb.AppendLine("• **Pharmacy**: Monday - Friday: 8:00 AM - 10:00 PM, Weekends: 9:00 AM - 8:00 PM\n");
            }

            if (message.Contains("contact") || message.Contains("phone") || message.Contains("email") ||
                message.Contains("address") || message.Contains("location") || message.Contains("where") ||
                message.Contains("reach") || message.Contains("call"))
            {
                sb.AppendLine("**Contact Us:**\n");
                sb.AppendLine("• **Main Phone**: (555) 123-4567\n");
                sb.AppendLine("• **Appointments**: (555) 123-4568\n");
                sb.AppendLine("• **Emergency**: (555) 123-4569\n");
                sb.AppendLine("• **Email**: info@novahealth.com\n");
                sb.AppendLine("• **Address**: 123 Medical Drive, Health City, HC 12345\n");
                sb.AppendLine("• **Emergency**: Call 911 or visit Emergency Department (24/7)\n");
                sb.AppendLine("• **Online Portal**: Access services through our patient portal\n");
            }

            sb.AppendLine("\n**For specific department locations and contact details:**");
            sb.AppendLine("Ask about a specific department by name, and I'll provide detailed information!");

            return sb.ToString();
        }

        private string HandleEmergencyQuery()
        {
            return "🚨 **Emergency Services**\n\n" +
                   "**For medical emergencies, please:**\n" +
                   "• **Call 911 immediately** for life-threatening situations\n" +
                   "• **Visit our Emergency Department** (open 24/7, 365 days)\n" +
                   "• **Location**: Ground floor, clearly marked Emergency entrance\n" +
                   "• **Staff**: Experienced medical professionals available 24/7\n\n" +
                   "**Emergency Symptoms (Call 911):**\n" +
                   "• Chest pain or pressure\n" +
                   "• Difficulty breathing\n" +
                   "• Severe bleeding\n" +
                   "• Sudden confusion or slurred speech\n" +
                   "• Severe allergic reaction\n" +
                   "• Unconsciousness\n\n" +
                   "**For urgent but non-life-threatening situations:**\n" +
                   "You can book an emergency appointment using:\n" +
                   "`add appointment doctorId=[ID] date=[DATE] time=[TIME] type=Emergency notes=[YOUR NOTES]`\n\n" +
                   "**Remember:** For true emergencies, always call 911 first!\n\n" +
                   "Our Emergency Department is fully equipped and staffed around the clock to provide immediate medical care.";
        }

        private string HandleGeneralInfoQuery()
        {
            var doctors = _doctorService.GetAllDoctors(false).ToList();
            var departments = _departmentService.GetAllDepartments(false).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("**About NovaHealth Hospital:**\n");
            sb.AppendLine("Welcome to NovaHealth Hospital, your trusted healthcare partner dedicated to providing exceptional medical care.\n");

            sb.AppendLine($"**Our Resources:**\n");
            sb.AppendLine($"• **{doctors.Count}** qualified and experienced doctors\n");
            sb.AppendLine($"• **{departments.Count}** specialized departments\n");
            sb.AppendLine("• **24/7 Emergency Services** - Always available\n");
            sb.AppendLine("• **Comprehensive Medical Care** - Full range of services\n");
            sb.AppendLine("• **Advanced Technology** - State-of-the-art medical equipment\n");
            sb.AppendLine("• **Patient-Centered Care** - Your health is our priority\n");
            sb.AppendLine("• **Telemedicine Services** - Virtual care options\n");
            sb.AppendLine("• **Research & Innovation** - Advancing medical science\n\n");

            sb.AppendLine("**Our Mission:**\n");
            sb.AppendLine("To provide high-quality, compassionate healthcare services to our community with excellence, integrity, and innovation.\n\n");

            sb.AppendLine("**Our Values:**\n");
            sb.AppendLine("• **Compassion** - Caring for every patient with empathy\n");
            sb.AppendLine("• **Excellence** - Delivering the highest standard of care\n");
            sb.AppendLine("• **Innovation** - Embracing new technologies and treatments\n");
            sb.AppendLine("• **Collaboration** - Working together for better health outcomes\n");
            sb.AppendLine("• **Integrity** - Maintaining the highest ethical standards\n\n");

            sb.AppendLine("**Would you like to know more about:**");
            sb.AppendLine("• Our doctors? Ask: 'List all doctors'");
            sb.AppendLine("• Our departments? Ask: 'Show departments'");
            sb.AppendLine("• Our services? Ask: 'What services do you offer?'");
            sb.AppendLine("• Booking appointments? Ask: 'How do I book an appointment?'");

            return sb.ToString();
        }

        private string HandleHelpQuery()
        {
            return "**How I Can Help You:**\n\n" +
                   "**1. Medical Information** 🩺\n" +
                   "• 'Symptoms of [condition]' - Learn about symptoms\n" +
                   "• 'Treatment for [condition]' - Common treatments\n" +
                   "• 'What causes [symptom]' - Possible causes\n" +
                   "• 'Health tips for [topic]' - Wellness advice\n\n" +

                   "**2. Find Doctors** 👨‍⚕️👩‍⚕️\n" +
                   "• 'List all doctors' - See all available doctors\n" +
                   "• 'Find doctors with specialization [name]' - Search by specialization\n" +
                   "• 'Show doctors in [department]' - Find doctors by department\n" +
                   "• 'Find doctor [name]' - Search for a specific doctor\n\n" +

                   "**3. Book Appointments** 📅\n" +
                   "• Use: 'add appointment doctorId=[ID] date=YYYY-MM-DD time=HH:mm type=[Type] notes=[Notes]'\n" +
                   "• Valid types: Consultation, FollowUp, Emergency, Surgery, Checkup\n" +
                   "• Ask: 'How do I book an appointment?' for detailed instructions\n\n" +

                   "**4. Hospital Information** 🏥\n" +
                   "• 'Show departments' - List all departments\n" +
                   "• 'Tell me about [department name]' - Get department details\n" +
                   "• 'What services do you offer?' - All services\n" +
                   "• 'What are your hours?' - Operating hours\n" +
                   "• 'How can I contact you?' - Contact information\n\n" +

                   "**5. Preventive Care** 🛡️\n" +
                   "• 'Preventive care recommendations' - Screening guidelines\n" +
                   "• 'Vaccination information' - Immunization schedules\n" +
                   "• 'Health screenings' - Test information\n\n" +

                   "**6. Medication Information** 💊\n" +
                   "• 'Common medications for [condition]' - Treatment options\n" +
                   "• 'How to take [medication]' - Usage instructions\n" +
                   "• 'Side effects of [medication]' - Safety information\n\n" +

                   "**7. Diagnostic Tests** 🔬\n" +
                   "• 'Information about [test name]' - Test details\n" +
                   "• 'How to prepare for [test]' - Preparation instructions\n" +
                   "• 'Understanding test results' - Result interpretation\n\n" +

                   "**8. Emergency Services** 🚨\n" +
                   "• Ask about emergency procedures\n" +
                   "• Get emergency contact information\n\n" +

                   "**Tips:**\n" +
                   "• Be specific in your questions for better results\n" +
                   "• You can combine queries\n" +
                   "• Need help? Just ask 'help' or 'what can you do?'\n\n" +

                   "What would you like to know?";
        }

        private string HandleThankYouOrGoodbye()
        {
            var responses = new[]
            {
                "You're very welcome! I'm glad I could help. Feel free to ask if you need anything else!",
                "You're welcome! It's my pleasure to assist you. Have a great day!",
                "Thank you for using NovaHealth Hospital services! Stay healthy and take care!",
                "You're welcome! If you have any more questions, I'm here to help. Goodbye!"
            };
            return responses[new Random().Next(responses.Length)];
        }

        private string HandlePreventiveCareQuery(string message)
        {
            var sb = new StringBuilder();
            sb.AppendLine("**Preventive Care & Screenings**\n");

            if (message.Contains("women") || message.Contains("female") || message.Contains("lady"))
            {
                sb.AppendLine("**Women's Preventive Care:**");
                foreach (var item in _preventiveCare["Women"])
                {
                    sb.AppendLine($"• {item}");
                }
            }
            else if (message.Contains("men") || message.Contains("male") || message.Contains("gentleman"))
            {
                sb.AppendLine("**Men's Preventive Care:**");
                foreach (var item in _preventiveCare["Men"])
                {
                    sb.AppendLine($"• {item}");
                }
            }
            else if (message.Contains("senior") || message.Contains("elderly") || message.Contains("older"))
            {
                sb.AppendLine("**Senior Preventive Care:**");
                foreach (var item in _preventiveCare["Seniors"])
                {
                    sb.AppendLine($"• {item}");
                }
            }
            else
            {
                sb.AppendLine("**General Preventive Care Recommendations:**");
                foreach (var item in _preventiveCare["General"])
                {
                    sb.AppendLine($"• {item}");
                }

                sb.AppendLine("\n**Age-Specific Recommendations:**");
                sb.AppendLine("• Ask about 'women's preventive care' for female-specific screenings");
                sb.AppendLine("• Ask about 'men's preventive care' for male-specific screenings");
                sb.AppendLine("• Ask about 'senior preventive care' for older adult screenings");
            }

            sb.AppendLine("\n**Importance of Preventive Care:**");
            sb.AppendLine("• Early detection of health issues");
            sb.AppendLine("• Better treatment outcomes");
            sb.AppendLine("• Reduced healthcare costs");
            sb.AppendLine("• Improved quality of life");
            sb.AppendLine("• Longer, healthier life");

            sb.AppendLine("\n**Schedule an appointment for preventive care today!**");

            return sb.ToString();
        }

        private string HandleMedicationQuery(string message)
        {
            var sb = new StringBuilder();
            sb.AppendLine("**Medication Information**\n");

            // Check for specific medication categories
            var mentionedCategories = _medicationInfo.Keys
                .Where(category => message.Contains(category.ToLower()))
                .ToList();

            if (mentionedCategories.Any())
            {
                foreach (var category in mentionedCategories.Take(2))
                {
                    sb.AppendLine($"**{category}:**");
                    foreach (var item in _medicationInfo[category])
                    {
                        sb.AppendLine($"• {item}");
                    }
                    sb.AppendLine();
                }
            }
            else
            {
                sb.AppendLine("**Common Medication Categories:**");
                foreach (var category in _medicationInfo.Keys)
                {
                    sb.AppendLine($"• {category}");
                }
                sb.AppendLine("\nAsk about a specific category for detailed information.");
            }

            sb.AppendLine("\n**Important Medication Safety Tips:**");
            sb.AppendLine("• Always follow your doctor's instructions");
            sb.AppendLine("• Never share medications");
            sb.AppendLine("• Store medications properly");
            sb.AppendLine("• Check expiration dates");
            sb.AppendLine("• Report side effects immediately");
            sb.AppendLine("• Don't stop medication without consulting your doctor");
            sb.AppendLine("• Keep a list of all medications you take");
            sb.AppendLine("• Inform all healthcare providers of your medications");

            sb.AppendLine("\n**Disclaimer:** This is general information. Always consult with a healthcare provider for medication advice.");

            return sb.ToString();
        }

        private string HandleDiagnosticQuery(string message)
        {
            var sb = new StringBuilder();
            sb.AppendLine("**Diagnostic Test Information**\n");

            // Check for specific tests mentioned
            var mentionedTests = _diagnosticInfo.Keys
                .Where(test => message.Contains(test.ToLower()))
                .ToList();

            if (mentionedTests.Any())
            {
                foreach (var test in mentionedTests.Take(2))
                {
                    sb.AppendLine($"**{test}:**");
                    sb.AppendLine(_diagnosticInfo[test]);
                    sb.AppendLine();
                }
            }
            else
            {
                sb.AppendLine("**Common Diagnostic Tests:**");
                foreach (var test in _diagnosticInfo.Keys)
                {
                    sb.AppendLine($"• {test}");
                }
                sb.AppendLine("\nAsk about a specific test for detailed information.");
            }

            sb.AppendLine("\n**Test Preparation Guidelines:**");
            sb.AppendLine("• Follow all preparation instructions");
            sb.AppendLine("• Inform your doctor of all medications");
            sb.AppendLine("• Mention allergies (especially to contrast dye)");
            sb.AppendLine("• Inform if you are or might be pregnant");
            sb.AppendLine("• Arrive on time for your appointment");
            sb.AppendLine("• Bring your insurance information");
            sb.AppendLine("• Wear comfortable, appropriate clothing");

            sb.AppendLine("\n**Getting Results:**");
            sb.AppendLine("• Results are typically available in 1-7 days");
            sb.AppendLine("• Your doctor will discuss results with you");
            sb.AppendLine("• Ask questions if you don't understand results");
            sb.AppendLine("• Keep copies of all test results");

            return sb.ToString();
        }

        private string HandleHealthTipQuery(string message)
        {
            var sb = new StringBuilder();
            sb.AppendLine("**Health & Wellness Tips**\n");

            // Check for specific health topics
            var mentionedTopics = _healthTips.Keys
                .Where(topic => message.Contains(topic.ToLower()))
                .ToList();

            if (mentionedTopics.Any())
            {
                foreach (var topic in mentionedTopics.Take(3))
                {
                    sb.AppendLine($"**{topic}:**");
                    sb.AppendLine(_healthTips[topic]);
                    sb.AppendLine();
                }
            }
            else
            {
                sb.AppendLine("**Health Topics:**");
                foreach (var topic in _healthTips.Keys)
                {
                    sb.AppendLine($"• {topic}");
                }
                sb.AppendLine("\nAsk about a specific topic for detailed information.");
            }

            sb.AppendLine("\n**Building Healthy Habits:**");
            sb.AppendLine("• Start small and build gradually");
            sb.AppendLine("• Set realistic goals");
            sb.AppendLine("• Track your progress");
            sb.AppendLine("• Find activities you enjoy");
            sb.AppendLine("• Get support from friends/family");
            sb.AppendLine("• Be patient with yourself");
            sb.AppendLine("• Celebrate your successes");

            sb.AppendLine("\n**Remember:** Small changes can lead to big health improvements over time!");

            return sb.ToString();
        }

        private string HandleSpecificDepartmentQuery(GetAllDepartmentsDto department)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"**{department.Name} Department**\n");

            if (!string.IsNullOrWhiteSpace(department.DepartmentSpeciality))
            {
                sb.AppendLine($"**Speciality:** {department.DepartmentSpeciality}\n");
            }

            if (!string.IsNullOrWhiteSpace(department.Location))
            {
                sb.AppendLine($"**Location:** {department.Location}\n");
            }

            if (!string.IsNullOrWhiteSpace(department.KeyServices))
            {
                sb.AppendLine($"**Key Services:**\n{department.KeyServices}\n");
            }

            // Find doctors in this department
            var doctors = _doctorService.GetAllDoctors(false)
                .Where(d => d.DepartmentId == department.Id)
                .Take(5)
                .ToList();

            if (doctors.Any())
            {
                sb.AppendLine("**Doctors in this Department:**\n");
                foreach (var doctor in doctors)
                {
                    sb.AppendLine($"• Dr. {doctor.Name} - {doctor.Specialization}");
                }
                sb.AppendLine();
            }

            sb.AppendLine("**Would you like to:**");
            sb.AppendLine("• See all doctors in this department? Ask: 'List doctors in [department name]'");
            sb.AppendLine("• Book an appointment? Use: 'add appointment doctorId=[ID] date=[DATE] time=[TIME] type=[TYPE] notes=[NOTES]'");

            return sb.ToString();
        }

        private string HandleSpecificDoctorQuery(GetAllDoctorsDto doctor)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"**Dr. {doctor.Name}**\n");
            sb.AppendLine($"**Specialization:** {doctor.Specialization}\n");

            if (!string.IsNullOrWhiteSpace(doctor.Department))
            {
                sb.AppendLine($"**Department:** {doctor.Department}\n");
            }

            sb.AppendLine($"**Contact Information:**");
            sb.AppendLine($"• Email: {doctor.Email}");
            sb.AppendLine($"• Phone: {doctor.PhoneNumber}");
            sb.AppendLine($"• Doctor ID: {doctor.Id}\n");

            sb.AppendLine("**To book an appointment with this doctor:**");
            sb.AppendLine($"`add appointment doctorId={doctor.Id} date=YYYY-MM-DD time=HH:mm type=[Type] notes=[Notes]`\n");

            sb.AppendLine("**Example:**");
            sb.AppendLine($"`add appointment doctorId={doctor.Id} date=2025-12-20 time=14:00 type=Consultation notes=Regular checkup`");

            return sb.ToString();
        }

        private string HandleDefaultResponse(string originalMessage, string lower)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"I understand you're asking about: \"{originalMessage}\"\n");

            // Try to provide helpful suggestions based on keywords
            var suggestions = new List<string>();

            if (lower.Contains("symptom") || lower.Contains("feel") || lower.Contains("pain"))
            {
                suggestions.Add("Try: 'Symptoms of [condition name]'");
                suggestions.Add("Try: 'What causes [symptom]'");
                suggestions.Add("Try: 'Treatment for [condition]'");
            }

            if (lower.Contains("doctor") || lower.Contains("find") || lower.Contains("search"))
            {
                suggestions.Add("Try: 'Find doctors with specialization [name]'");
                suggestions.Add("Try: 'List all doctors'");
                suggestions.Add("Try: 'Show departments'");
            }

            if (lower.Contains("appointment") || lower.Contains("book") || lower.Contains("schedule"))
            {
                suggestions.Add("Try: 'How do I book an appointment?'");
                suggestions.Add("Try: 'add appointment doctorId=[ID] date=[DATE] time=[TIME]'");
            }

            if (lower.Contains("test") || lower.Contains("scan") || lower.Contains("x-ray"))
            {
                suggestions.Add("Try: 'Information about [test name]'");
                suggestions.Add("Try: 'How to prepare for [test]'");
            }

            if (lower.Contains("medicine") || lower.Contains("medication") || lower.Contains("pill"))
            {
                suggestions.Add("Try: 'Common medications for [condition]'");
                suggestions.Add("Try: 'How to take [medication]'");
            }

            if (!suggestions.Any())
            {
                suggestions.Add("'Symptoms of [condition]' - Medical information");
                suggestions.Add("'List all doctors' - Find available doctors");
                suggestions.Add("'Show departments' - See all departments");
                suggestions.Add("'How do I book an appointment?' - Appointment guide");
                suggestions.Add("'What services do you offer?' - See all services");
                suggestions.Add("'Health tips for [topic]' - Wellness advice");
            }

            sb.AppendLine("I can help you with:\n");
            sb.AppendLine("• Medical information about symptoms and conditions\n");
            sb.AppendLine("• Finding doctors by specialization or department\n");
            sb.AppendLine("• Booking appointments\n");
            sb.AppendLine("• Information about our departments and services\n");
            sb.AppendLine("• Hospital hours and contact information\n");
            sb.AppendLine("• Preventive care and screenings\n");
            sb.AppendLine("• Medication information\n");
            sb.AppendLine("• Diagnostic test information\n");
            sb.AppendLine("• Health and wellness tips\n\n");

            sb.AppendLine("**Try asking:**\n");
            foreach (var suggestion in suggestions.Take(5))
            {
                sb.AppendLine($"• {suggestion}");
            }
            sb.AppendLine();
            sb.AppendLine("**Or use the appointment command:**");
            sb.AppendLine("`add appointment doctorId=[ID] date=[DATE] time=[TIME] type=[TYPE] notes=[NOTES]`\n");
            sb.AppendLine("Need help? Just ask 'help' or 'what can you do?'");

            return sb.ToString();
        }

        #endregion

        #region Helper Methods

        private string? ExtractSpecialization(string message)
        {
            var specializationKeywords = new[] { "specialization", "speciality", "specialize", "specialist", "expertise", "category" };
            var commonSpecializations = new[]
            {
                "cardiology", "cardiac", "heart", "pediatrics", "pediatric", "orthopedics", "orthopedic",
                "neurology", "neurological", "surgery", "surgical", "dermatology", "dermatological",
                "psychiatry", "psychiatric", "oncology", "cancer", "radiology", "radiological",
                "gastroenterology", "gi", "ophthalmology", "eye", "ent", "otolaryngology"
            };

            foreach (var keyword in specializationKeywords)
            {
                var index = message.IndexOf(keyword);
                if (index >= 0)
                {
                    var afterKeyword = message.Substring(index + keyword.Length).Trim();
                    var parts = afterKeyword.Split(new[] { " is ", "=", ":", ",", ".", " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0)
                    {
                        var spec = parts[0].Trim();
                        if (spec.Length > 2) return spec;
                    }
                }
            }

            foreach (var spec in commonSpecializations)
            {
                if (message.Contains(spec)) return spec;
            }

            return null;
        }

        private string? ExtractDepartmentName(string message)
        {
            var departmentKeywords = new[] { "department", "dept", "division", "unit", "clinic" };
            var departments = _departmentService.GetAllDepartments(false).ToList();

            foreach (var keyword in departmentKeywords)
            {
                var index = message.IndexOf(keyword);
                if (index >= 0)
                {
                    var afterKeyword = message.Substring(index + keyword.Length).Trim();
                    var parts = afterKeyword.Split(new[] { " is ", "=", ":", ",", ".", " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0)
                    {
                        var deptName = parts[0].Trim();
                        if (deptName.Length > 2)
                        {
                            var match = departments.FirstOrDefault(d =>
                                d.Name.Contains(deptName, StringComparison.OrdinalIgnoreCase));
                            if (match != null) return match.Name;
                        }
                    }
                }
            }

            foreach (var dept in departments)
            {
                if (message.Contains(dept.Name, StringComparison.OrdinalIgnoreCase) ||
                    message.Contains(dept.DepartmentSpeciality, StringComparison.OrdinalIgnoreCase))
                {
                    return dept.Name;
                }
            }

            return null;
        }

        private GetAllDepartmentsDto? TryFindSpecificDepartment(string message)
        {
            var departments = _departmentService.GetAllDepartments(false).ToList();

            foreach (var dept in departments)
            {
                if (message.Contains(dept.Name, StringComparison.OrdinalIgnoreCase) ||
                    message.Contains(dept.DepartmentSpeciality, StringComparison.OrdinalIgnoreCase))
                {
                    return dept;
                }
            }

            return null;
        }

        private GetAllDoctorsDto? TryFindSpecificDoctor(string message)
        {
            var doctors = _doctorService.GetAllDoctors(false).ToList();

            foreach (var doctor in doctors)
            {
                if (message.Contains(doctor.Name, StringComparison.OrdinalIgnoreCase) ||
                    message.Contains(doctor.FirstName, StringComparison.OrdinalIgnoreCase) ||
                    message.Contains(doctor.LastName, StringComparison.OrdinalIgnoreCase))
                {
                    return doctor;
                }
            }

            return null;
        }

        #endregion
    }
}