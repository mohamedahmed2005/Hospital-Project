using Hospital.BBL.DTOs.AppointmentDTOs;
using Hospital.BBL.DTOs.DepartmentDTOs;
using Hospital.BBL.DTOs.DoctorDTOs;
using Hospital.BBL.Services.Interfaces;
using Hospital.DAL.Models.AppointmentModule;
using Hospital.DAL.Models.Shared;
using Hospital.PL.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.RegularExpressions;

namespace Hospital.PL.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ChatController : Controller
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IDoctorService _doctorService;
        private readonly IAppointmentService _appointmentService;
        private readonly IDepartmentService _departmentService;
        private readonly IPatientService _patientService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LocalChatService _localChatService;

        // Static data containers
        private readonly Dictionary<string, List<string>> _symptomsToConditions;
        private readonly Dictionary<string, List<string>> _conditionsToMedicines;
        private readonly Dictionary<string, List<string>> _specializedTreatments;
        private readonly Dictionary<string, List<string>> _generalAdvice;
        private readonly Dictionary<string, List<string>> _dietaryAdvice;
        private readonly Dictionary<string, List<string>> _exerciseRecommendations;
        private readonly Dictionary<string, List<string>> _mentalHealthTips;
        private readonly HashSet<string> _greetings;
        private readonly HashSet<string> _farewells;
        private readonly Dictionary<string, string> _contactInfo;
        private readonly Dictionary<string, List<string>> _vaccinationSchedule;
        private readonly Dictionary<string, List<string>> _firstAidProcedures;
        private readonly Dictionary<string, List<string>> _commonLabTests;

        public ChatController(
            ILogger<ChatController> logger,
            IDoctorService doctorService,
            IAppointmentService appointmentService,
            IDepartmentService departmentService,
            IPatientService patientService,
            UserManager<ApplicationUser> userManager,
            LocalChatService localChatService)
        {
            _logger = logger;
            _doctorService = doctorService;
            _appointmentService = appointmentService;
            _departmentService = departmentService;
            _patientService = patientService;
            _userManager = userManager;
            _localChatService = localChatService;

            // Initialize static data
            _symptomsToConditions = InitializeSymptomsDatabase();
            _conditionsToMedicines = InitializeMedicineDatabase();
            _specializedTreatments = InitializeSpecializedTreatments();
            _generalAdvice = InitializeGeneralAdvice();
            _dietaryAdvice = InitializeDietaryAdvice();
            _exerciseRecommendations = InitializeExerciseRecommendations();
            _mentalHealthTips = InitializeMentalHealthTips();
            _greetings = InitializeGreetings();
            _farewells = InitializeFarewells();
            _contactInfo = InitializeContactInfo();
            _vaccinationSchedule = InitializeVaccinationSchedule();
            _firstAidProcedures = InitializeFirstAidProcedures();
            _commonLabTests = InitializeCommonLabTests();
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

        #region Static Data Initialization

        private Dictionary<string, List<string>> InitializeSymptomsDatabase()
        {
            return new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["headache"] = new List<string> { "Migraine", "Tension Headache", "Sinusitis", "Dehydration", "Eye Strain", "Cluster Headache", "Hormonal Headache", "Medication Overuse" },
                ["fever"] = new List<string> { "Common Cold", "Flu", "COVID-19", "UTI", "Pneumonia", "Appendicitis", "Meningitis", "Sepsis" },
                ["cough"] = new List<string> { "Bronchitis", "Asthma", "Common Cold", "Allergies", "COVID-19", "Pneumonia", "GERD", "Pertussis" },
                ["fatigue"] = new List<string> { "Anemia", "Thyroid Issues", "Depression", "Sleep Apnea", "CFS", "Diabetes", "Heart Disease", "Autoimmune Disorders" },
                ["nausea"] = new List<string> { "Food Poisoning", "Gastroenteritis", "Migraine", "Pregnancy", "Motion Sickness", "Medication Side Effects", "GERD", "Pancreatitis" },
                ["chest pain"] = new List<string> { "Angina", "Heart Attack", "Costochondritis", "Panic Attack", "GERD", "Pulmonary Embolism", "Pneumonia", "Pericarditis" },
                ["shortness of breath"] = new List<string> { "Asthma", "Pneumonia", "Anxiety", "Heart Failure", "COPD", "Anemia", "Pulmonary Edema", "Pneumothorax" },
                ["abdominal pain"] = new List<string> { "Appendicitis", "Gallstones", "IBS", "Gastritis", "Food Poisoning", "Ulcers", "Pancreatitis", "Diverticulitis" },
                ["joint pain"] = new List<string> { "Arthritis", "Gout", "Lupus", "Tendonitis", "Bursitis", "Fibromyalgia", "Lyme Disease", "Osteoporosis" },
                ["dizziness"] = new List<string> { "Vertigo", "Low BP", "Dehydration", "Inner Ear Infection", "Anemia", "Migraine", "Heart Arrhythmia", "Medication Effects" },
                ["sore throat"] = new List<string> { "Strep Throat", "Common Cold", "Tonsillitis", "Allergies", "GERD", "Mononucleosis", "Smoking", "Dry Air" },
                ["rash"] = new List<string> { "Eczema", "Psoriasis", "Allergic Reaction", "Contact Dermatitis", "Shingles", "Rosacea", "Hives", "Fungal Infection" },
                ["back pain"] = new List<string> { "Muscle Strain", "Herniated Disc", "Sciatica", "Arthritis", "Kidney Infection", "Osteoporosis", "Spinal Stenosis", "Poor Posture" },
                ["insomnia"] = new List<string> { "Anxiety", "Depression", "Sleep Apnea", "Restless Leg Syndrome", "Chronic Pain", "Caffeine Intake", "Irregular Schedule" },
                ["weight loss"] = new List<string> { "Hyperthyroidism", "Diabetes", "Cancer", "Depression", "Eating Disorders", "GI Disorders", "Tuberculosis" },
                ["palpitations"] = new List<string> { "Anxiety", "Arrhythmia", "Anemia", "Hyperthyroidism", "Electrolyte Imbalance", "Heart Disease", "Medication Effects" }
            };
        }

        private Dictionary<string, List<string>> InitializeMedicineDatabase()
        {
            return new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Migraine"] = new List<string> { "Sumatriptan (Imitrex)", "Rizatriptan (Maxalt)", "Acetaminophen", "Ibuprofen", "Naproxen", "Propranolol", "Topiramate", "Amitriptyline" },
                ["Common Cold"] = new List<string> { "Acetaminophen", "Ibuprofen", "Pseudoephedrine", "Dextromethorphan", "Zinc Lozenges", "Vitamin C", "Chlorpheniramine", "Guaifenesin" },
                ["Flu"] = new List<string> { "Oseltamivir", "Zanamivir", "Acetaminophen", "Ibuprofen", "Rest", "Hydration", "Baloxavir", "Peramivir" },
                ["Bronchitis"] = new List<string> { "Albuterol", "Guaifenesin", "Dextromethorphan", "Azithromycin", "Cough Drops", "Prednisone", "Codeine", "Amoxicillin" },
                ["Asthma"] = new List<string> { "Albuterol", "Fluticasone", "Montelukast", "Prednisone", "Levalbuterol", "Salmeterol", "Budesonide", "Theophylline" },
                ["Arthritis"] = new List<string> { "Ibuprofen", "Naproxen", "Celecoxib", "Methotrexate", "Physical Therapy", "Hydroxychloroquine", "Sulfasalazine", "Biologics" },
                ["Gastritis"] = new List<string> { "Omeprazole", "Famotidine", "Sucralfate", "Antacids", "Diet Modification", "Ranitidine", "Esomeprazole", "Misoprostol" },
                ["Anxiety"] = new List<string> { "Sertraline", "Escitalopram", "Buspirone", "CBT", "Breathing Exercises", "Alprazolam", "Diazepam", "Propranolol" },
                ["Hypertension"] = new List<string> { "Lisinopril", "Amlodipine", "Metoprolol", "Hydrochlorothiazide", "Lifestyle Changes", "Losartan", "Valsartan", "Diltiazem" },
                ["Diabetes"] = new List<string> { "Metformin", "Insulin", "Glipizide", "Diet Control", "Exercise", "Sitagliptin", "Empagliflozin", "Liraglutide" },
                ["Depression"] = new List<string> { "Fluoxetine", "Bupropion", "Venlafaxine", "Therapy", "Exercise", "Citalopram", "Duloxetine", "Mirtazapine" },
                ["Allergies"] = new List<string> { "Loratadine", "Cetirizine", "Fexofenadine", "Fluticasone Nasal Spray", "Avoid Allergens", "Diphenhydramine", "Montelukast", "Immunotherapy" }
            };
        }

        private Dictionary<string, List<string>> InitializeSpecializedTreatments()
        {
            return new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Cancer"] = new List<string> { "Chemotherapy", "Radiation Therapy", "Immunotherapy", "Targeted Therapy", "Surgery", "Hormone Therapy", "Stem Cell Transplant" },
                ["Heart Disease"] = new List<string> { "Angioplasty", "Bypass Surgery", "Stent Placement", "Pacemaker", "Medication Management", "Cardiac Rehabilitation" },
                ["Stroke"] = new List<string> { "Clot-busting Drugs", "Thrombectomy", "Rehabilitation", "Physical Therapy", "Speech Therapy", "Occupational Therapy" },
                ["Diabetes"] = new List<string> { "Insulin Therapy", "Oral Medications", "Blood Sugar Monitoring", "Dietary Management", "Exercise Program", "Continuous Glucose Monitor" },
                ["Kidney Disease"] = new List<string> { "Dialysis", "Kidney Transplant", "Medication Management", "Dietary Restrictions", "Blood Pressure Control" },
                ["Liver Disease"] = new List<string> { "Liver Transplant", "Medication Therapy", "Dietary Management", "Avoid Alcohol", "Regular Monitoring" },
                ["Neurological Disorders"] = new List<string> { "Medication Management", "Physical Therapy", "Occupational Therapy", "Speech Therapy", "Surgical Interventions" }
            };
        }

        private Dictionary<string, List<string>> InitializeGeneralAdvice()
        {
            return new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["general"] = new List<string>
                {
                    "Stay hydrated - drink at least 8 glasses of water daily",
                    "Get 7-9 hours of quality sleep each night",
                    "Exercise for at least 30 minutes most days",
                    "Eat a balanced diet with plenty of fruits and vegetables",
                    "Manage stress through meditation or yoga",
                    "Don't skip regular health check-ups",
                    "Practice good hygiene and hand washing",
                    "Limit alcohol and avoid smoking",
                    "Maintain a healthy weight for your height",
                    "Wear sunscreen when outdoors",
                    "Practice safe food handling",
                    "Get vaccinated as recommended"
                },
                ["emergency"] = new List<string>
                {
                    "For chest pain or difficulty breathing, call emergency immediately",
                    "Severe bleeding that won't stop requires immediate attention",
                    "Sudden confusion or difficulty speaking could be a stroke",
                    "Severe allergic reactions need emergency treatment",
                    "If you suspect poisoning, call poison control immediately",
                    "Head injuries with loss of consciousness need urgent care",
                    "Seizures lasting more than 5 minutes require emergency care",
                    "Severe burns covering large areas need immediate attention"
                },
                ["prevention"] = new List<string>
                {
                    "Get vaccinated according to schedule",
                    "Wear seatbelts and helmets when appropriate",
                    "Use sunscreen to protect against skin cancer",
                    "Practice safe food handling and cooking",
                    "Get regular cancer screenings as recommended",
                    "Maintain a healthy weight",
                    "Limit processed foods and added sugars",
                    "Practice safe sex",
                    "Avoid sharing personal items",
                    "Keep up with dental checkups",
                    "Monitor blood pressure regularly",
                    "Practice fall prevention at home"
                }
            };
        }

        private Dictionary<string, List<string>> InitializeDietaryAdvice()
        {
            return new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["general"] = new List<string>
                {
                    "Eat plenty of colorful vegetables daily",
                    "Choose whole grains over refined grains",
                    "Include lean protein sources",
                    "Limit saturated and trans fats",
                    "Reduce sodium intake",
                    "Control portion sizes",
                    "Eat regular meals throughout the day",
                    "Limit added sugars",
                    "Stay hydrated with water",
                    "Read food labels carefully"
                },
                ["specific"] = new List<string>
                {
                    "For hypertension: DASH diet, low sodium",
                    "For diabetes: Carb counting, low glycemic index",
                    "For heart health: Mediterranean diet",
                    "For kidney disease: Low potassium, low phosphorus",
                    "For weight loss: Calorie control, high protein"
                }
            };
        }

        private Dictionary<string, List<string>> InitializeExerciseRecommendations()
        {
            return new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["general"] = new List<string>
                {
                    "150 minutes moderate exercise weekly",
                    "Include strength training 2x weekly",
                    "Add flexibility exercises",
                    "Incorporate balance training",
                    "Start slowly if new to exercise",
                    "Listen to your body",
                    "Warm up before exercise",
                    "Cool down after exercise",
                    "Stay hydrated during exercise",
                    "Use proper equipment"
                },
                ["specific"] = new List<string>
                {
                    "For arthritis: Low-impact exercises",
                    "For back pain: Core strengthening",
                    "For heart health: Cardio exercises",
                    "For diabetes: Regular aerobic activity",
                    "For osteoporosis: Weight-bearing exercises"
                }
            };
        }

        private Dictionary<string, List<string>> InitializeMentalHealthTips()
        {
            return new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["general"] = new List<string>
                {
                    "Practice mindfulness meditation",
                    "Maintain social connections",
                    "Set realistic goals",
                    "Practice gratitude daily",
                    "Get adequate sleep",
                    "Limit screen time",
                    "Engage in hobbies",
                    "Seek professional help when needed",
                    "Practice deep breathing",
                    "Take regular breaks"
                },
                ["coping"] = new List<string>
                {
                    "Identify triggers",
                    "Develop healthy coping mechanisms",
                    "Practice relaxation techniques",
                    "Challenge negative thoughts",
                    "Set boundaries",
                    "Practice self-care",
                    "Join support groups",
                    "Keep a journal"
                }
            };
        }

        private HashSet<string> InitializeGreetings()
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "hello", "hi", "hey", "good morning", "good afternoon", "good evening",
                "greetings", "howdy", "what's up", "yo", "hi there", "hello there",
                "good day", "morning", "afternoon", "evening", "sup", "hiya", "hey there",
                "hello friend", "good to see you", "welcome", "salutations", "how are you",
                "how do you do", "pleased to meet you", "nice to meet you", "long time no see"
            };
        }

        private HashSet<string> InitializeFarewells()
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "bye", "goodbye", "see you", "farewell", "take care", "later",
                "ciao", "adios", "so long", "bye bye", "have a good day",
                "good night", "night", "talk to you later", "catch you later",
                "until next time", "peace out", "signing off", "see you later",
                "take it easy", "be well", "stay safe", "until we meet again",
                "have a nice day", "all the best", "cheerio", "ta-ta"
            };
        }

        private Dictionary<string, string> InitializeContactInfo()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["phone_main"] = "(555) 123-4567",
                ["phone_appointments"] = "(555) 123-4568",
                ["phone_emergency"] = "(555) 123-4569",
                ["phone_fax"] = "(555) 123-4570",
                ["phone_pharmacy"] = "(555) 123-4571",
                ["phone_billing"] = "(555) 123-4572",
                ["phone_telemedicine"] = "(555) 123-4573",
                ["email_general"] = "info@novahealth.com",
                ["email_appointments"] = "appointments@novahealth.com",
                ["email_billing"] = "billing@novahealth.com",
                ["email_support"] = "support@novahealth.com",
                ["email_medical_records"] = "records@novahealth.com",
                ["email_pharmacy"] = "pharmacy@novahealth.com",
                ["address_main"] = "123 Medical Drive, Health City, HC 12345",
                ["address_south_campus"] = "456 Wellness Blvd, Health City, HC 12346",
                ["address_north_clinic"] = "789 Care Avenue, Health City, HC 12347",
                ["hours_main"] = "Mon-Fri 7AM-9PM • Sat-Sun 8AM-6PM",
                ["hours_emergency"] = "24/7 Emergency Services",
                ["hours_pharmacy"] = "Mon-Fri 8AM-10PM • Sat-Sun 9AM-8PM",
                ["hours_telemedicine"] = "24/7 Virtual Consultations",
                ["parking"] = "Free parking available",
                ["valet"] = "Valet parking available at main entrance",
                ["wheelchair"] = "Wheelchair accessible throughout campus",
                ["interpreter"] = "Interpreter services available",
                ["social_facebook"] = "facebook.com/novahealth",
                ["social_twitter"] = "twitter.com/novahealth",
                ["social_instagram"] = "instagram.com/novahealth"
            };
        }

        private Dictionary<string, List<string>> InitializeVaccinationSchedule()
        {
            return new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Infants"] = new List<string> { "Hepatitis B", "DTaP", "Hib", "Polio", "PCV13", "Rotavirus", "MMR", "Varicella", "Hepatitis A" },
                ["Children"] = new List<string> { "DTaP", "IPV", "MMR", "Varicella", "Hepatitis A", "HPV", "Meningococcal", "Tdap", "Influenza" },
                ["Adolescents"] = new List<string> { "HPV", "Meningococcal", "Tdap", "Influenza", "COVID-19", "Hepatitis B series" },
                ["Adults"] = new List<string> { "Influenza", "Tdap", "MMR", "Varicella", "Shingles", "Pneumococcal", "COVID-19", "Hepatitis" },
                ["Seniors"] = new List<string> { "Influenza", "Pneumococcal", "Shingles", "Tdap", "COVID-19" }
            };
        }

        private Dictionary<string, List<string>> InitializeFirstAidProcedures()
        {
            return new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Burns"] = new List<string> { "Cool with running water", "Cover with sterile dressing", "Do not apply ice", "Do not pop blisters", "Seek medical help for severe burns" },
                ["Cuts"] = new List<string> { "Clean wound", "Apply pressure", "Use sterile dressing", "Elevate if possible", "Seek stitches for deep cuts" },
                ["Choking"] = new List<string> { "Perform Heimlich maneuver", "Call emergency", "Check airway", "Perform back blows", "Seek medical help" },
                ["CPR"] = new List<string> { "Check responsiveness", "Call emergency", "Start chest compressions", "Give rescue breaths", "Use AED if available" },
                ["Fractures"] = new List<string> { "Immobilize area", "Apply ice", "Elevate if possible", "Do not try to realign", "Seek medical help" },
                ["Heat Stroke"] = new List<string> { "Move to cool area", "Cool with water", "Fan the person", "Monitor breathing", "Call emergency" }
            };
        }

        private Dictionary<string, List<string>> InitializeCommonLabTests()
        {
            return new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Blood"] = new List<string> { "CBC", "Chemistry Panel", "Lipid Profile", "Blood Sugar", "Thyroid Panel", "Liver Function", "Kidney Function", "Iron Studies" },
                ["Urine"] = new List<string> { "Urinalysis", "Urine Culture", "Microalbumin", "Pregnancy Test", "Drug Screen", "24-hour Collection" },
                ["Imaging"] = new List<string> { "X-Ray", "CT Scan", "MRI", "Ultrasound", "Mammogram", "Bone Density", "PET Scan" },
                ["Cardiac"] = new List<string> { "EKG", "Stress Test", "Echocardiogram", "Holter Monitor", "Cardiac Enzymes" },
                ["Cancer"] = new List<string> { "PSA", "CA-125", "CEA", "AFP", "Biopsy", "Pap Smear", "Colonoscopy" }
            };
        }

        #endregion

        #region Response Handlers

        private string HandleGreeting()
        {
            var greetings = new[]
            {
                "Hello! I'm NovaHealth Assistant. How can I help you today? 😊",
                "Hi there! Welcome to NovaHealth Hospital's virtual assistant. What can I do for you?",
                "Good day! I'm here to assist with medical information, appointments, and general inquiries.",
                "Hello! Ready to help with your health questions and hospital services.",
                "Welcome! I'm your NovaHealth assistant. How may I assist you today?"
            };
            return greetings[new Random().Next(greetings.Length)];
        }

        private string HandleFarewell()
        {
            var farewells = new[]
            {
                "Goodbye! Take care of your health! 🏥",
                "Farewell! Remember to stay hydrated and get enough rest!",
                "Take care! Don't hesitate to contact us if you need medical assistance.",
                "Goodbye! Wishing you good health and wellness!",
                "See you later! Remember, your health is your wealth! 💙"
            };
            return farewells[new Random().Next(farewells.Length)];
        }

        private string HandleContactInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine("📞 **Contact NovaHealth Hospital**");
            sb.AppendLine();
            sb.AppendLine("**Phone & Fax**");
            sb.AppendLine($"Main Line: {_contactInfo["phone_main"]}");
            sb.AppendLine($"Appointments: {_contactInfo["phone_appointments"]}");
            sb.AppendLine($"Emergency: {_contactInfo["phone_emergency"]}");
            sb.AppendLine($"Fax: {_contactInfo["phone_fax"]}");
            sb.AppendLine($"Pharmacy: {_contactInfo["phone_pharmacy"]}");
            sb.AppendLine($"Billing: {_contactInfo["phone_billing"]}");
            sb.AppendLine($"Telemedicine: {_contactInfo["phone_telemedicine"]}");
            sb.AppendLine();
            sb.AppendLine("**Email & Digital**");
            sb.AppendLine($"General: {_contactInfo["email_general"]}");
            sb.AppendLine($"Appointments: {_contactInfo["email_appointments"]}");
            sb.AppendLine($"Billing: {_contactInfo["email_billing"]}");
            sb.AppendLine($"Support: {_contactInfo["email_support"]}");
            sb.AppendLine($"Medical Records: {_contactInfo["email_medical_records"]}");
            sb.AppendLine($"Pharmacy: {_contactInfo["email_pharmacy"]}");
            sb.AppendLine();
            sb.AppendLine("**Visit Us**");
            sb.AppendLine($"Main Campus: {_contactInfo["address_main"]}");
            sb.AppendLine($"South Campus: {_contactInfo["address_south_campus"]}");
            sb.AppendLine($"North Clinic: {_contactInfo["address_north_clinic"]}");
            sb.AppendLine($"Hours: {_contactInfo["hours_main"]}");
            sb.AppendLine($"Emergency: {_contactInfo["hours_emergency"]}");
            sb.AppendLine($"Pharmacy: {_contactInfo["hours_pharmacy"]}");
            sb.AppendLine($"Telemedicine: {_contactInfo["hours_telemedicine"]}");
            sb.AppendLine();
            sb.AppendLine("**Facilities**");
            sb.AppendLine($"{_contactInfo["parking"]}");
            sb.AppendLine($"{_contactInfo["valet"]}");
            sb.AppendLine($"{_contactInfo["wheelchair"]}");
            sb.AppendLine($"{_contactInfo["interpreter"]}");
            sb.AppendLine();
            sb.AppendLine("**Connect With Us**");
            sb.AppendLine($"Facebook: {_contactInfo["social_facebook"]}");
            sb.AppendLine($"Twitter: {_contactInfo["social_twitter"]}");
            sb.AppendLine($"Instagram: {_contactInfo["social_instagram"]}");
            sb.AppendLine();
            sb.AppendLine("We're here to help you 24/7!");

            return sb.ToString();
        }

        private string HandleSymptomsCheck(string message)
        {
            var lowerMessage = message.ToLowerInvariant();
            var detectedSymptoms = new List<string>();

            foreach (var symptom in _symptomsToConditions.Keys)
            {
                if (lowerMessage.Contains(symptom, StringComparison.OrdinalIgnoreCase))
                {
                    detectedSymptoms.Add(symptom);
                }
            }

            if (detectedSymptoms.Count == 0)
            {
                return "I don't recognize specific symptoms in your message. Could you describe what you're feeling in more detail?";
            }

            var sb = new StringBuilder();
            sb.AppendLine("Based on the symptoms you mentioned, here's what I found:");
            sb.AppendLine();

            foreach (var symptom in detectedSymptoms.Take(3))
            {
                sb.AppendLine($"**{symptom.ToUpperInvariant()}**");
                sb.AppendLine($"Possible conditions: {string.Join(", ", _symptomsToConditions[symptom].Take(3))}");

                // Add first aid advice
                sb.AppendLine("**Immediate Care:**");
                switch (symptom.ToLower())
                {
                    case "fever":
                        sb.AppendLine("- Rest and stay hydrated");
                        sb.AppendLine("- Monitor temperature regularly");
                        sb.AppendLine("- Seek medical help if fever exceeds 103°F (39.4°C)");
                        break;
                    case "headache":
                        sb.AppendLine("- Rest in a dark, quiet room");
                        sb.AppendLine("- Stay hydrated");
                        sb.AppendLine("- Consider over-the-counter pain relief");
                        break;
                    case "chest pain":
                        sb.AppendLine("- Call emergency immediately");
                        sb.AppendLine("- Sit down and rest");
                        sb.AppendLine("- Do not drive yourself to hospital");
                        break;
                    case "shortness of breath":
                        sb.AppendLine("- Sit upright");
                        sb.AppendLine("- Try to stay calm");
                        sb.AppendLine("- Seek immediate medical attention");
                        break;
                    default:
                        sb.AppendLine("- Rest and monitor symptoms");
                        sb.AppendLine("- Stay hydrated");
                        sb.AppendLine("- Seek medical advice if symptoms worsen");
                        break;
                }
                sb.AppendLine();
            }

            sb.AppendLine("**When to Seek Emergency Care:**");
            foreach (var advice in _generalAdvice["emergency"].Take(4))
            {
                sb.AppendLine($"• {advice}");
            }
            sb.AppendLine();

            sb.AppendLine("⚠️ **Important Disclaimer:**");
            sb.AppendLine("This information is for educational purposes only. Please consult a healthcare professional for proper diagnosis and treatment.");
            sb.AppendLine($"For emergencies, call: {_contactInfo["phone_emergency"]}");

            return sb.ToString();
        }

        private string HandleMedicineSuggestions(string message)
        {
            var lowerMessage = message.ToLowerInvariant();
            var detectedConditions = new List<string>();

            foreach (var condition in _conditionsToMedicines.Keys)
            {
                if (lowerMessage.Contains(condition, StringComparison.OrdinalIgnoreCase))
                {
                    detectedConditions.Add(condition);
                }
            }

            if (detectedConditions.Count == 0)
            {
                return "I don't recognize specific medical conditions in your message. Could you specify which condition you're asking about?";
            }

            var sb = new StringBuilder();
            sb.AppendLine("**Common Treatment Options**");
            sb.AppendLine("⚠️ **Important:** Always consult with a healthcare provider before taking any medication.");
            sb.AppendLine();

            foreach (var condition in detectedConditions.Take(2))
            {
                sb.AppendLine($"**For {condition.ToUpperInvariant()}:**");
                sb.AppendLine("Commonly prescribed medications may include:");

                foreach (var medicine in _conditionsToMedicines[condition])
                {
                    sb.AppendLine($"• {medicine}");
                }

                sb.AppendLine();
                sb.AppendLine("**General Management Tips:**");
                sb.AppendLine("- Follow your doctor's prescription exactly");
                sb.AppendLine("- Report any side effects immediately");
                sb.AppendLine("- Don't stop medication without consulting your doctor");
                sb.AppendLine("- Keep regular follow-up appointments");

                // Add dietary and lifestyle advice
                if (_dietaryAdvice.ContainsKey("specific"))
                {
                    sb.AppendLine("- Follow appropriate dietary guidelines");
                }
                sb.AppendLine();
            }

            sb.AppendLine("**Remember:** Self-medication can be dangerous. Always seek professional medical advice.");

            return sb.ToString();
        }

        private string HandleGeneralAdvice(string message)
        {
            var lowerMessage = message.ToLowerInvariant();
            var sb = new StringBuilder();

            sb.AppendLine("**Health & Wellness Tips**");
            sb.AppendLine();

            // Check for specific advice categories
            if (lowerMessage.Contains("emergency") || lowerMessage.Contains("urgent"))
            {
                sb.AppendLine("🚨 **Emergency Guidelines:**");
                foreach (var advice in _generalAdvice["emergency"])
                {
                    sb.AppendLine($"• {advice}");
                }
                sb.AppendLine();
            }

            if (lowerMessage.Contains("prevent") || lowerMessage.Contains("avoid"))
            {
                sb.AppendLine("🛡️ **Prevention Tips:**");
                foreach (var advice in _generalAdvice["prevention"])
                {
                    sb.AppendLine($"• {advice}");
                }
                sb.AppendLine();
            }

            if (lowerMessage.Contains("diet") || lowerMessage.Contains("food") || lowerMessage.Contains("eat"))
            {
                sb.AppendLine("🥗 **Dietary Recommendations:**");
                foreach (var advice in _dietaryAdvice["general"])
                {
                    sb.AppendLine($"• {advice}");
                }
                sb.AppendLine();
            }

            if (lowerMessage.Contains("exercise") || lowerMessage.Contains("workout") || lowerMessage.Contains("fitness"))
            {
                sb.AppendLine("💪 **Exercise Guidelines:**");
                foreach (var advice in _exerciseRecommendations["general"])
                {
                    sb.AppendLine($"• {advice}");
                }
                sb.AppendLine();
            }

            if (lowerMessage.Contains("mental") || lowerMessage.Contains("stress") || lowerMessage.Contains("anxiety"))
            {
                sb.AppendLine("🧠 **Mental Health Tips:**");
                foreach (var advice in _mentalHealthTips["general"])
                {
                    sb.AppendLine($"• {advice}");
                }
                sb.AppendLine();
            }

            // Always include general advice
            sb.AppendLine("💪 **General Health Tips:**");
            foreach (var advice in _generalAdvice["general"])
            {
                sb.AppendLine($"• {advice}");
            }

            sb.AppendLine();
            sb.AppendLine("For personalized advice, please consult with our healthcare professionals.");

            return sb.ToString();
        }

        private string HandleServicesInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine("🏥 **NovaHealth Hospital Services**");
            sb.AppendLine();
            sb.AppendLine("**Medical Departments:**");
            sb.AppendLine("• Emergency Medicine (24/7)");
            sb.AppendLine("• Cardiology & Heart Care");
            sb.AppendLine("• Neurology & Neurosurgery");
            sb.AppendLine("• Orthopedics & Sports Medicine");
            sb.AppendLine("• Pediatrics & Child Health");
            sb.AppendLine("• Oncology & Cancer Care");
            sb.AppendLine("• Radiology & Imaging");
            sb.AppendLine("• Laboratory Services");
            sb.AppendLine("• Physical Therapy & Rehabilitation");
            sb.AppendLine("• Mental Health & Psychiatry");
            sb.AppendLine("• Dermatology");
            sb.AppendLine("• Gastroenterology");
            sb.AppendLine("• Ophthalmology");
            sb.AppendLine("• ENT (Ear, Nose & Throat)");
            sb.AppendLine();
            sb.AppendLine("**Specialized Services:**");
            sb.AppendLine("• Telemedicine Consultations");
            sb.AppendLine("• Health Check-up Packages");
            sb.AppendLine("• Vaccination Programs");
            sb.AppendLine("• Mental Health Services");
            sb.AppendLine("• Nutritional Counseling");
            sb.AppendLine("• Senior Care Programs");
            sb.AppendLine("• Maternity & Childbirth");
            sb.AppendLine("• Surgical Services");
            sb.AppendLine("• Pharmacy Services");
            sb.AppendLine("• Home Health Care");
            sb.AppendLine();
            sb.AppendLine("**Patient Support:**");
            sb.AppendLine("• Insurance Assistance");
            sb.AppendLine("• Medical Records Access");
            sb.AppendLine("• Transportation Services");
            sb.AppendLine("• Interpreter Services");
            sb.AppendLine("• Financial Counseling");
            sb.AppendLine("• Patient Education");
            sb.AppendLine();
            sb.AppendLine("**Diagnostic Services:**");
            foreach (var test in _commonLabTests["Imaging"])
            {
                sb.AppendLine($"• {test}");
            }
            sb.AppendLine();
            sb.AppendLine("Type 'doctors' to see available doctors or 'appointment' to book a consultation.");

            return sb.ToString();
        }

        private string HandleVaccinationInfo(string message)
        {
            var lowerMessage = message.ToLowerInvariant();
            var sb = new StringBuilder();

            sb.AppendLine("💉 **Vaccination Information**");
            sb.AppendLine();

            if (lowerMessage.Contains("baby") || lowerMessage.Contains("infant"))
            {
                sb.AppendLine("**Infant Vaccination Schedule:**");
                foreach (var vaccine in _vaccinationSchedule["Infants"])
                {
                    sb.AppendLine($"• {vaccine}");
                }
            }
            else if (lowerMessage.Contains("child") || lowerMessage.Contains("kid"))
            {
                sb.AppendLine("**Childhood Vaccination Schedule:**");
                foreach (var vaccine in _vaccinationSchedule["Children"])
                {
                    sb.AppendLine($"• {vaccine}");
                }
            }
            else if (lowerMessage.Contains("adult"))
            {
                sb.AppendLine("**Adult Vaccination Schedule:**");
                foreach (var vaccine in _vaccinationSchedule["Adults"])
                {
                    sb.AppendLine($"• {vaccine}");
                }
            }
            else if (lowerMessage.Contains("senior") || lowerMessage.Contains("elderly"))
            {
                sb.AppendLine("**Senior Vaccination Schedule:**");
                foreach (var vaccine in _vaccinationSchedule["Seniors"])
                {
                    sb.AppendLine($"• {vaccine}");
                }
            }
            else
            {
                sb.AppendLine("**Recommended Vaccinations by Age Group:**");
                sb.AppendLine();
                foreach (var group in _vaccinationSchedule.Keys)
                {
                    sb.AppendLine($"**{group}:**");
                    sb.AppendLine(string.Join(", ", _vaccinationSchedule[group].Take(3)));
                    sb.AppendLine();
                }
            }

            sb.AppendLine();
            sb.AppendLine("**Important Notes:**");
            sb.AppendLine("• Always consult with your healthcare provider");
            sb.AppendLine("• Keep a vaccination record");
            sb.AppendLine("• Report any side effects immediately");
            sb.AppendLine($"• Contact our pharmacy: {_contactInfo["phone_pharmacy"]}");

            return sb.ToString();
        }

        private string HandleFirstAidInfo(string message)
        {
            var lowerMessage = message.ToLowerInvariant();
            var sb = new StringBuilder();

            sb.AppendLine("🩹 **First Aid Information**");
            sb.AppendLine();

            foreach (var procedure in _firstAidProcedures.Keys)
            {
                if (lowerMessage.Contains(procedure.ToLower()))
                {
                    sb.AppendLine($"**For {procedure}:**");
                    foreach (var step in _firstAidProcedures[procedure])
                    {
                        sb.AppendLine($"• {step}");
                    }
                    sb.AppendLine();
                }
            }

            if (sb.Length == "🩹 **First Aid Information**\n\n".Length)
            {
                sb.AppendLine("**Common First Aid Procedures:**");
                foreach (var procedure in _firstAidProcedures.Keys)
                {
                    sb.AppendLine($"• {procedure}");
                }
                sb.AppendLine();
                sb.AppendLine("Ask about a specific procedure for detailed steps.");
            }

            sb.AppendLine();
            sb.AppendLine("🚨 **Emergency Contact:**");
            sb.AppendLine($"Call {_contactInfo["phone_emergency"]} for immediate assistance");
            sb.AppendLine("Call 911 for life-threatening emergencies");

            return sb.ToString();
        }

        private string HandleLabTestInfo(string message)
        {
            var lowerMessage = message.ToLowerInvariant();
            var sb = new StringBuilder();

            sb.AppendLine("🔬 **Laboratory Test Information**");
            sb.AppendLine();

            foreach (var category in _commonLabTests.Keys)
            {
                if (lowerMessage.Contains(category.ToLower()))
                {
                    sb.AppendLine($"**{category} Tests:**");
                    foreach (var test in _commonLabTests[category])
                    {
                        sb.AppendLine($"• {test}");
                    }
                    sb.AppendLine();
                }
            }

            if (sb.Length == "🔬 **Laboratory Test Information**\n\n".Length)
            {
                sb.AppendLine("**Common Test Categories:**");
                foreach (var category in _commonLabTests.Keys)
                {
                    sb.AppendLine($"• {category}");
                }
                sb.AppendLine();
                sb.AppendLine("Ask about a specific test category for more information.");
            }

            sb.AppendLine();
            sb.AppendLine("**Test Preparation:**");
            sb.AppendLine("• Follow your doctor's instructions");
            sb.AppendLine("• Fasting may be required for some tests");
            sb.AppendLine("• Bring your insurance information");
            sb.AppendLine("• Arrive 15 minutes before your appointment");

            return sb.ToString();
        }

        #endregion

        #region Existing Methods

        private string GetDoctorsSummary(string? specializationFilter = null, string? departmentFilter = null)
        {
            var doctorsQuery = _doctorService.GetAllDoctors(false);

            if (!string.IsNullOrWhiteSpace(specializationFilter))
            {
                doctorsQuery = doctorsQuery
                    .Where(d => !string.IsNullOrWhiteSpace(d.Specialization) &&
                                d.Specialization.Contains(specializationFilter, StringComparison.OrdinalIgnoreCase));
            }

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
            sb.AppendLine();

            foreach (var d in doctors)
            {
                var departmentInfo = !string.IsNullOrWhiteSpace(d.Department) ? $" - Department: {d.Department}" : "";
                sb.AppendLine($"• **{d.Name}** - {d.Specialization}{departmentInfo}");
            }

            if (doctors.Count >= 10)
            {
                sb.AppendLine($"\n(Showing first 10 doctors. There may be more available.)");
            }

            sb.AppendLine("\nTo book an appointment with any doctor, use: `add appointment doctorId=X date=YYYY-MM-DD time=HH:MM`");
            sb.AppendLine("\nType 'services' to see all our hospital services.");

            return sb.ToString();
        }

        private (string? specialization, string? department) ExtractDoctorFilters(string message)
        {
            var lower = message.ToLowerInvariant();
            string? specialization = null;
            string? department = null;

            var specializationKeywords = new[] { "specialization", "speciality", "category", "specialize", "expertise" };
            var departmentKeywords = new[] { "department", "dept", "division", "unit" };

            foreach (var keyword in specializationKeywords)
            {
                var index = lower.IndexOf(keyword);
                if (index >= 0)
                {
                    var afterKeyword = message.Substring(index + keyword.Length).Trim();
                    var parts = afterKeyword.Split(new[] { " is ", "=", ":", ",", ".", " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0)
                    {
                        specialization = parts[0].Trim();
                        specialization = specialization.Replace("that", "").Replace("its", "").Replace("the", "").Trim();
                        if (specialization.Length > 2)
                            break;
                    }
                }
            }

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
            var lower = message.ToLowerInvariant();
            if (!lower.StartsWith("add appointment") && !lower.StartsWith("book appointment") &&
                !lower.StartsWith("create appointment") && !lower.StartsWith("schedule appointment"))
            {
                return new ChatResponse { Error = null, Reply = string.Empty };
            }

            try
            {
                if (!User.Identity?.IsAuthenticated ?? true)
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = "You must be logged in to create an appointment. Please log in first."
                    };
                }

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

                var matches = Regex.Matches(message, @"(\w+)=(""[^""]*""|\S+)");
                var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                foreach (Match m in matches)
                {
                    var key = m.Groups[1].Value;
                    var value = m.Groups[2].Value;

                    if (value.StartsWith("\"") && value.EndsWith("\""))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    dict[key] = value;
                }

                if (!dict.TryGetValue("doctorId", out var doctorIdStr) ||
                    !dict.TryGetValue("date", out var dateStr) ||
                    !dict.TryGetValue("time", out var timeStr))
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = "📋 **To book an appointment:**\n" +
                               "Use: `add appointment doctorId=2 date=2025-12-20 time=14:00 type=Consultation notes=\"Your notes here\"`\n\n" +
                               "**Example:** `add appointment doctorId=5 date=2025-01-15 time=10:30 type=Follow-up notes=\"Routine check-up\"`\n\n" +
                               "**Need help?** Type 'doctors' to see available doctors or 'contact' for assistance."
                    };
                }

                if (!int.TryParse(doctorIdStr, out int doctorId) || doctorId <= 0)
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = "Invalid doctor ID. Please provide a valid positive number."
                    };
                }

                var doctor = _doctorService.GetDoctorById(doctorId);
                if (doctor == null)
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = $"❌ Doctor with ID {doctorId} not found.\nType 'doctors' to see available doctors."
                    };
                }

                if (!DateOnly.TryParse(dateStr, out var date))
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = "Invalid date format. Please use: YYYY-MM-DD (e.g., 2025-12-20)"
                    };
                }

                if (date < DateOnly.FromDateTime(DateTime.Now))
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = "Cannot book appointments for past dates. Please select a future date."
                    };
                }

                if (!TimeSpan.TryParse(timeStr, out var time))
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = "Invalid time format. Please use: HH:mm (e.g., 14:00 or 2:00 PM)"
                    };
                }

                var existingAppointments = _appointmentService.GetAppointmentsByDoctorAndDate(doctorId, date);
                var conflictingAppointment = existingAppointments.FirstOrDefault(a =>
                    a.Appointment_Time == time && a.Status != AppointmentStatus.cancelled);

                if (conflictingAppointment != null)
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = $"⏰ Doctor #{doctorId} is already booked on {date:yyyy-MM-dd} at {time:hh\\:mm}.\nPlease choose a different time."
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
                    : "Appointment booked via NovaHealth Assistant.";

                if (notes.Length < 3)
                {
                    return new ChatResponse
                    {
                        Error = null,
                        Reply = "Please provide meaningful notes (at least 3 characters)."
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
                        Reply = "Failed to create appointment. Please try again or contact support."
                    };
                }

                return new ChatResponse
                {
                    Error = null,
                    Reply = $"✅ **Appointment Confirmed!**\n\n" +
                           $"**Appointment ID:** #{addedId}\n" +
                           $"**Patient:** {patient.Name}\n" +
                           $"**Doctor:** Dr. {doctor.Name} ({doctor.Specialization})\n" +
                           $"**Date:** {date:dddd, MMMM dd, yyyy}\n" +
                           $"**Time:** {time:hh\\:mm}\n" +
                           $"**Type:** {appointmentType}\n\n" +
                           $"📅 **Reminder:** Arrive 15 minutes before your appointment.\n" +
                           $"📞 Need to reschedule? Call: {_contactInfo["phone_appointments"]}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating appointment from chat command.");
                return new ChatResponse
                {
                    Error = null,
                    Reply = $"❌ Error: {ex.Message}\n\nFor assistance, contact: {_contactInfo["phone_appointments"]}"
                };
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
                return BadRequest(new ChatResponse { Error = "Please type a message." });

            var lower = request.Message.ToLowerInvariant();
            var trimmed = request.Message.Trim();

            // 1) Check for greetings
            if (_greetings.Any(g => lower.Contains(g)))
            {
                return Ok(new ChatResponse { Reply = HandleGreeting() });
            }

            // 2) Check for farewells
            if (_farewells.Any(f => lower.Contains(f)))
            {
                return Ok(new ChatResponse { Reply = HandleFarewell() });
            }

            // 3) Check for contact information requests
            if (lower.Contains("contact") || lower.Contains("phone") || lower.Contains("email") ||
                lower.Contains("address") || lower.Contains("call") || lower.Contains("reach"))
            {
                return Ok(new ChatResponse { Reply = HandleContactInfo() });
            }

            // 4) Check for services information
            if (lower.Contains("services") || lower.Contains("departments") || lower.Contains("what do you offer"))
            {
                return Ok(new ChatResponse { Reply = HandleServicesInfo() });
            }

            // 5) Check for vaccination information
            if (lower.Contains("vaccine") || lower.Contains("vaccination") || lower.Contains("immunization"))
            {
                return Ok(new ChatResponse { Reply = HandleVaccinationInfo(request.Message) });
            }

            // 6) Check for first aid information
            if (lower.Contains("first aid") || lower.Contains("emergency care") || lower.Contains("cpr"))
            {
                return Ok(new ChatResponse { Reply = HandleFirstAidInfo(request.Message) });
            }

            // 7) Check for lab test information
            if (lower.Contains("test") || lower.Contains("lab") || lower.Contains("blood test") || lower.Contains("x-ray"))
            {
                return Ok(new ChatResponse { Reply = HandleLabTestInfo(request.Message) });
            }

            // 8) Check for symptom descriptions
            if (lower.Contains("feel") || lower.Contains("symptom") || lower.Contains("pain") ||
                lower.Contains("hurt") || lower.Contains("ache") ||
                _symptomsToConditions.Keys.Any(s => lower.Contains(s)))
            {
                return Ok(new ChatResponse { Reply = HandleSymptomsCheck(request.Message) });
            }

            // 9) Check for medicine inquiries
            if (lower.Contains("medicine") || lower.Contains("medication") || lower.Contains("pill") ||
                lower.Contains("drug") || lower.Contains("treatment for") ||
                _conditionsToMedicines.Keys.Any(c => lower.Contains(c)))
            {
                return Ok(new ChatResponse { Reply = HandleMedicineSuggestions(request.Message) });
            }

            // 10) Check for general health advice
            if (lower.Contains("advice") || lower.Contains("tip") || lower.Contains("recommend") ||
                lower.Contains("should i") || lower.Contains("what to do") || lower.Contains("how to"))
            {
                return Ok(new ChatResponse { Reply = HandleGeneralAdvice(request.Message) });
            }

            // 11) Check for doctor queries
            if (lower.Contains("doctor") || lower.Contains("doctors"))
            {
                var isDoctorQuery = lower.Contains("list") || lower.Contains("show") ||
                                   lower.Contains("find") || lower.Contains("get") ||
                                   lower.Contains("display") || lower.Contains("see") ||
                                   lower.Contains("available") || lower.Contains("which doctor");

                if (isDoctorQuery)
                {
                    var (specialization, department) = ExtractDoctorFilters(request.Message);
                    var doctorsText = GetDoctorsSummary(specialization, department);
                    return Ok(new ChatResponse { Reply = doctorsText });
                }
            }

            // 12) Check for appointment commands
            var appointmentResult = await TryHandleAppointmentCommandAsync(request.Message);
            if (!string.IsNullOrWhiteSpace(appointmentResult.Reply) && string.IsNullOrEmpty(appointmentResult.Error))
            {
                return Ok(appointmentResult);
            }

            // 13) Help command
            if (lower.Contains("help") || lower == "?" || lower == "what can you do")
            {
                var helpText = @"🤖 **NovaHealth Assistant Help**

I can help you with:

**Appointments:**
• Book appointments: `add appointment doctorId=X date=YYYY-MM-DD time=HH:MM`
• See available doctors: `show doctors`

**Medical Information:**
• Describe symptoms for possible conditions
• Get general health advice and tips
• Learn about common treatments
• Vaccination information
• First aid procedures
• Lab test information

**Hospital Services:**
• View all departments and services: `services`
• Get contact information: `contact`
• Learn about our facilities

**Health & Wellness:**
• Dietary recommendations
• Exercise guidelines
• Mental health tips
• Prevention strategies

**General:**
• Greet me with: `hello`, `hi`
• Say goodbye: `bye`, `goodbye`
• Get emergency info: `emergency`

**Examples:**
• `I have headache and fever`
• `What medicine for cold?`
• `Show me cardiology doctors`
• `How to prevent flu?`
• `Vaccination schedule for children`
• `First aid for burns`
• `What blood tests are available?`

Type your question or choose from above options!";

                return Ok(new ChatResponse { Reply = helpText });
            }

            // 14) Emergency keyword detection
            if (lower.Contains("emergency") || lower.Contains("urgent") ||
                lower.Contains("911") || lower.Contains("help me") ||
                lower.Contains("dying") || lower.Contains("heart attack") ||
                lower.Contains("stroke") || lower.Contains("bleeding"))
            {
                var emergencyText = $"🚨 **EMERGENCY ALERT** 🚨\n\n" +
                                  $"If this is a medical emergency:\n" +
                                  $"1. Call {_contactInfo["phone_emergency"]} immediately\n" +
                                  $"2. Go to the nearest emergency room\n" +
                                  $"3. Don't wait for a response here\n\n" +
                                  $"**NovaHealth Emergency Department**\n" +
                                  $"{_contactInfo["address_main"]}\n" +
                                  $"Open 24/7\n\n" +
                                  $"For non-emergencies, describe your symptoms and I'll try to help.";

                return Ok(new ChatResponse { Reply = emergencyText });
            }

            // 15) Use local chat service for everything else
            try
            {
                var reply = _localChatService.GenerateResponse(request.Message);
                return Ok(new ChatResponse { Reply = reply });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while generating local chat response.");
                return Ok(new ChatResponse
                {
                    Reply = $"I'm having trouble understanding that. Could you rephrase?\n\n" +
                           $"For specific help, try:\n" +
                           $"• `help` - See what I can do\n" +
                           $"• `contact` - Get contact info\n" +
                           $"• `services` - View hospital services"
                });
            }
        }

        #endregion
    }
}