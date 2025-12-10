(function () {
    function getCurrentLanguage() {
        return localStorage.getItem("language") === "ar" ? "ar" : "en";
    }

    const translations = {
        en: {
            title: "Nova Assistant",
            status: "Online • AI assistant",
            statusThinking: "Thinking...",
            placeholder: "Ask Nova a question...",
            welcome: "Hi, I'm Nova, your AI assistant. I can help with NovaHealth departments, doctors, appointments, and general questions.",
            error: "Sorry, something went wrong talking to the AI.",
            errorNetwork: "Network error while contacting AI service.",
            help: "I didn't understand that, but I'm here to help.",
            buttonTitle: "Chat with Nova",
            quickActions: "Quick Actions",
            clearChat: "Clear Chat",
            listening: "Listening... Speak now",
            stopListening: "Stop Listening",
            typeMessage: "Type your message...",
            noMessages: "No messages yet. Start a conversation!",
            deleteConfirm: "Are you sure you want to delete all chat history? This action cannot be undone.",
            deleteTitle: "Delete All"
        },
        ar: {
            title: "مساعد نوفا",
            status: "متصل • مساعد ذكي",
            statusThinking: "جاري التفكير...",
            placeholder: "اطرح سؤالاً على نوفا...",
            welcome: "مرحباً، أنا نوفا، مساعدك الذكي. يمكنني المساعدة في أقسام نوفا هيلث والأطباء والمواعيد والأسئلة العامة.",
            error: "عذراً، حدث خطأ أثناء التحدث مع الذكي.",
            errorNetwork: "خطأ في الشبكة أثناء الاتصال بخدمة الذكي.",
            help: "لم أفهم ذلك، لكنني هنا للمساعدة.",
            buttonTitle: "التحدث مع نوفا",
            quickActions: "إجراءات سريعة",
            clearChat: "مسح المحادثة",
            listening: "جاري الاستماع... تحدث الآن",
            stopListening: "إيقاف الاستماع",
            typeMessage: "اكتب رسالتك...",
            noMessages: "لا توجد رسائل بعد. ابدأ محادثة!",
            deleteConfirm: "هل أنت متأكد أنك تريد حذف سجل المحادثة بالكامل؟ لا يمكن التراجع عن هذا الإجراء.",
            deleteTitle: "حذف الكل"
        }
    };

    const quickActions = {
        en: [
            { text: "🏥 Departments", value: "Tell me about NovaHealth departments" },
            { text: "👨‍⚕️ Doctors", value: "Show me available doctors and their specialties" },
            { text: "📅 Appointments", value: "How do I book an appointment?" },
            { text: "🚨 Emergency", value: "Emergency services and contact information" },
            { text: "ℹ️ About", value: "Tell me about NovaHealth hospital" },
            { text: "📍 Locations", value: "NovaHealth hospital locations" }
        ],
        ar: [
            { text: "🏥 الأقسام", value: "Tell me about NovaHealth departments" },
            { text: "👨‍⚕️ الأطباء", value: "Show me available doctors and their specialties" },
            { text: "📅 المواعيد", value: "How do I book an appointment?" },
            { text: "🚨 الطوارئ", value: "Emergency services and contact information" },
            { text: "ℹ️ عن نوفا", value: "Tell me about NovaHealth hospital" },
            { text: "📍 المواقع", value: "NovaHealth hospital locations" }
        ]
    };

    const root = document.createElement("div");
    root.id = "nova-chatbot-root";
    root.innerHTML = `
<style>
    #nova-chatbot-root {
        position: fixed;
        bottom: calc(2rem + 50px + 10px);
        z-index: 999999;
        font-family: "Poppins", system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif;
        direction: ltr;
    }
    
    body.ltr #nova-chatbot-root {
        right: 2rem;
        left: auto;
    }
    
    body.rtl #nova-chatbot-root {
        left: 2rem;
        right: auto;
    }

    .nova-chatbot-button {
        width: 60px;
        height: 60px;
        border-radius: 50%;
        border: none;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: #fff;
        display: flex;
        align-items: center;
        justify-content: center;
        box-shadow: 0 8px 32px rgba(102, 126, 234, 0.4);
        cursor: pointer;
        transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
        position: relative;
        overflow: hidden;
    }

    .nova-chatbot-button::before {
        content: '';
        position: absolute;
        width: 100%;
        height: 100%;
        border-radius: 50%;
        background: rgba(255, 255, 255, 0.1);
        animation: wave 2s linear infinite;
    }

    .nova-chatbot-button::after {
        content: '';
        position: absolute;
        width: 100%;
        height: 100%;
        border-radius: 50%;
        background: rgba(255, 255, 255, 0.05);
        animation: wave 2s linear infinite 0.5s;
    }

    @keyframes wave {
        0% {
            transform: scale(1);
            opacity: 1;
        }
        100% {
            transform: scale(1.5);
            opacity: 0;
        }
    }

    .nova-chatbot-button:hover {
        transform: scale(1.1) rotate(5deg);
        box-shadow: 0 12px 40px rgba(102, 126, 234, 0.6);
    }

    .nova-chatbot-button i {
        font-size: 1.5rem;
        z-index: 1;
        transition: transform 0.3s ease;
    }

    .nova-chatbot-button:hover i {
        transform: scale(1.1);
    }

    .nova-chatbot-button.recording {
        background: linear-gradient(135deg, #f56565 0%, #ed64a6 100%);
        animation: pulse 1.5s infinite;
    }

    @keyframes pulse {
        0% {
            box-shadow: 0 0 0 0 rgba(245, 101, 101, 0.7);
        }
        70% {
            box-shadow: 0 0 0 10px rgba(245, 101, 101, 0);
        }
        100% {
            box-shadow: 0 0 0 0 rgba(245, 101, 101, 0);
        }
    }

    .nova-chatbot-window {
        position: fixed;
        bottom: 130px;
        width: 400px;
        max-width: calc(100vw - 40px);
        height: 600px;
        max-height: calc(100vh - 200px);
        background: #ffffff;
        border-radius: 20px;
        box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
        display: flex;
        flex-direction: column;
        overflow: hidden;
        opacity: 0;
        transform: translateY(20px) scale(0.95);
        transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
        direction: ltr;
        border: 1px solid rgba(255, 255, 255, 0.2);
        visibility: hidden;
        z-index: 999999;
    }

    body.ltr .nova-chatbot-window {
        right: 85px;
        left: auto;
    }

    body.rtl .nova-chatbot-window {
        left: 85px;
        right: auto;
    }

    .nova-chatbot-window.show {
        opacity: 1;
        transform: translateY(0) scale(1);
        visibility: visible;
    }

    body.rtl .nova-chatbot-window {
        direction: rtl;
    }

    /* FIX: Mobile positioning - Make Arabic same as English */
   @media (max-width: 768px) {
    /* Fix for both English and Arabic - make window centered */
    .nova-chatbot-window {
        width: calc(100vw - 2rem);
        max-width: calc(100vw - 2rem);
        height: 70vh;
        max-height: 70vh;
        bottom: 70px;
        /* Center the window horizontally for both languages */
        left: 50% !important;
        right: auto !important;
        transform: translateX(-50%) translateY(20px) scale(0.95);
        margin: 0;
        border-radius: 16px;
    }

    /* Ensure both LTR and RTL windows are centered */
    body.ltr .nova-chatbot-window,
    body.rtl .nova-chatbot-window {
        left: 50% !important;
        right: auto !important;
        transform: translateX(-50%) translateY(20px) scale(0.95);
    }

    .nova-chatbot-window.show {
        transform: translateX(-50%) translateY(0) scale(1);
    }

    body.ltr .nova-chatbot-window.show,
    body.rtl .nova-chatbot-window.show {
        transform: translateX(-50%) translateY(0) scale(1);
    }

    /* Keep button position consistent for both languages */
    #nova-chatbot-root {
        bottom: calc(2rem + 50px + 10px);
        right: 1rem !important;
        left: auto !important;
    }

    body.rtl #nova-chatbot-root {
        right: 1rem !important;
        left: auto !important;
    }

    .nova-chatbot-button {
        width: 56px;
        height: 56px;
    }

    .nova-chatbot-quick-grid {
        grid-template-columns: repeat(2, 1fr);
    }

    .nova-chatbot-bubble {
        max-width: 90%;
    }
}

  @media (max-width: 480px) {
    /* Keep button position */
    #nova-chatbot-root {
        bottom: calc(2rem + 50px + 10px);
        right: 0.75rem !important;
        left: auto !important;
    }

    body.rtl #nova-chatbot-root {
        right: 0.75rem !important;
        left: auto !important;
    }

    /* Window remains centered */
    .nova-chatbot-window {
        width: calc(100vw - 1.5rem);
        max-width: calc(100vw - 1.5rem);
        height: 65vh;
        max-height: 65vh;
        bottom: 60px;
        left: 50% !important;
        right: auto !important;
        transform: translateX(-50%) translateY(20px) scale(0.95);
    }

    body.ltr .nova-chatbot-window,
    body.rtl .nova-chatbot-window {
        left: 50% !important;
        right: auto !important;
        transform: translateX(-50%) translateY(20px) scale(0.95);
    }

    .nova-chatbot-window.show {
        transform: translateX(-50%) translateY(0) scale(1);
    }

    body.ltr .nova-chatbot-window.show,
    body.rtl .nova-chatbot-window.show {
        transform: translateX(-50%) translateY(0) scale(1);
    }

    .nova-chatbot-quick-grid {
        grid-template-columns: 1fr;
    }

    .nova-chatbot-bubble {
        max-width: 95%;
        font-size: 0.85rem;
    }

    .nova-chatbot-footer {
        padding: 0.75rem;
    }

    .nova-chatbot-footer textarea {
        font-size: 0.85rem;
        padding: 0.6rem 0.75rem;
    }
}

    /* Keep the rest of your original CSS */
    .nova-chatbot-header {
        padding: 1rem 1.25rem;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: #fff;
        display: flex;
        align-items: center;
        justify-content: space-between;
        position: relative;
        overflow: hidden;
    }

    .nova-chatbot-header::before {
        content: '';
        position: absolute;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background: linear-gradient(45deg, transparent 30%, rgba(255,255,255,0.1) 50%, transparent 70%);
        animation: shimmer 3s infinite;
    }

    @keyframes shimmer {
        0% { transform: translateX(-100%); }
        100% { transform: translateX(100%); }
    }

    .nova-chatbot-header-title {
        display: flex;
        align-items: center;
        gap: 0.75rem;
        font-weight: 600;
        z-index: 1;
    }

    .nova-chatbot-header-title i {
        font-size: 1.5rem;
        filter: drop-shadow(0 2px 4px rgba(0,0,0,0.2));
    }

    .nova-chatbot-status {
        font-size: 0.8rem;
        opacity: 0.9;
        font-weight: 400;
        margin-top: 2px;
    }

    .nova-chatbot-header-controls {
        display: flex;
        gap: 0.5rem;
        z-index: 1;
    }

    .nova-chatbot-btn-icon {
        width: 32px;
        height: 32px;
        border-radius: 50%;
        border: none;
        background: rgba(255, 255, 255, 0.2);
        color: white;
        display: flex;
        align-items: center;
        justify-content: center;
        cursor: pointer;
        transition: all 0.2s ease;
    }

    .nova-chatbot-btn-icon:hover {
        background: rgba(255, 255, 255, 0.3);
        transform: scale(1.1);
    }

    .nova-chatbot-btn-icon.recording {
        background: rgba(245, 101, 101, 0.3);
        animation: recordingPulse 1s infinite;
    }

    .nova-chatbot-btn-icon.recording i {
        animation: micPulse 0.5s infinite;
    }

    @keyframes recordingPulse {
        0%, 100% { transform: scale(1); }
        50% { transform: scale(1.1); }
    }

    @keyframes micPulse {
        0%, 100% { transform: scale(1); }
        50% { transform: scale(1.2); }
    }

    .nova-chatbot-body {
        flex: 1;
        overflow-y: auto;
        padding: 1rem;
        background: linear-gradient(180deg, #f8fafc 0%, #f1f5f9 100%);
        position: relative;
        display: flex;
        flex-direction: column;
    }

    .nova-chatbot-quick-actions {
        margin-bottom: 1rem;
        animation: slideUp 0.5s ease;
    }

    @keyframes slideUp {
        from {
            opacity: 0;
            transform: translateY(10px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }

    .nova-chatbot-quick-title {
        font-size: 0.85rem;
        color: #64748b;
        margin-bottom: 0.75rem;
        font-weight: 500;
        display: flex;
        align-items: center;
        gap: 0.5rem;
    }

    .nova-chatbot-quick-grid {
        display: grid;
        grid-template-columns: repeat(2, 1fr);
        gap: 0.5rem;
    }

    .nova-chatbot-quick-btn {
        padding: 0.6rem 0.75rem;
        background: white;
        border: 1px solid #e2e8f0;
        border-radius: 10px;
        font-size: 0.8rem;
        color: #475569;
        cursor: pointer;
        transition: all 0.2s ease;
        text-align: center;
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 0.5rem;
        border: none;
        box-shadow: 0 2px 4px rgba(0,0,0,0.05);
    }

    .nova-chatbot-quick-btn:hover {
        background: linear-gradient(135deg, #f0f4ff 0%, #f8fafc 100%);
        border-color: #667eea;
        color: #667eea;
        transform: translateY(-2px);
        box-shadow: 0 4px 12px rgba(102, 126, 234, 0.1);
    }

    .nova-chatbot-message {
        margin-bottom: 1rem;
        display: flex;
        animation: messageAppear 0.3s ease;
    }

    @keyframes messageAppear {
        from {
            opacity: 0;
            transform: translateY(10px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }

    .nova-chatbot-message.user {
        justify-content: flex-end;
    }

    body.rtl .nova-chatbot-message.user {
        justify-content: flex-start;
    }

    .nova-chatbot-message.assistant {
        justify-content: flex-start;
    }

    body.rtl .nova-chatbot-message.assistant {
        justify-content: flex-end;
    }

    .nova-chatbot-bubble {
        max-width: 85%;
        padding: 0.75rem 1rem;
        border-radius: 18px;
        font-size: 0.9rem;
        line-height: 1.5;
        word-wrap: break-word;
        position: relative;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
    }

    .nova-chatbot-message.user .nova-chatbot-bubble {
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: #fff;
        border-bottom-right-radius: 4px;
        border-bottom-left-radius: 18px;
    }

    body.rtl .nova-chatbot-message.user .nova-chatbot-bubble {
        border-bottom-right-radius: 18px;
        border-bottom-left-radius: 4px;
    }

    .nova-chatbot-message.assistant .nova-chatbot-bubble {
        background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
        color: #1e293b;
        border: 1px solid #e2e8f0;
        border-bottom-left-radius: 4px;
        border-bottom-right-radius: 18px;
    }

    body.rtl .nova-chatbot-message.assistant .nova-chatbot-bubble {
        border-bottom-left-radius: 18px;
        border-bottom-right-radius: 4px;
    }

    .nova-chatbot-footer {
        padding: 1rem;
        border-top: 1px solid #e2e8f0;
        background: white;
        display: flex;
        gap: 0.75rem;
        align-items: flex-end;
    }

    body.rtl .nova-chatbot-footer {
        flex-direction: row-reverse;
    }

    .nova-chatbot-footer textarea {
        flex: 1;
        min-height: 44px;
        max-height: 120px;
        padding: 0.75rem 1rem;
        border: 1px solid #e2e8f0;
        border-radius: 12px;
        font-size: 0.9rem;
        resize: none;
        background: #f8fafc;
        transition: all 0.2s ease;
        direction: ltr;
        text-align: left;
        font-family: inherit;
    }

    .nova-chatbot-footer textarea:focus {
        outline: none;
        border-color: #667eea;
        background: white;
        box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
    }

    body.rtl .nova-chatbot-footer textarea {
        direction: rtl;
        text-align: right;
    }

    .nova-chatbot-footer-actions {
        display: flex;
        gap: 0.5rem;
        align-items: center;
    }

    .nova-chatbot-btn-primary {
        width: 44px;
        height: 44px;
        border-radius: 12px;
        border: none;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: white;
        display: flex;
        align-items: center;
        justify-content: center;
        cursor: pointer;
        transition: all 0.2s ease;
    }

    .nova-chatbot-btn-primary:hover {
        transform: translateY(-2px);
        box-shadow: 0 6px 20px rgba(102, 126, 234, 0.3);
    }

    .nova-chatbot-btn-primary:disabled {
        opacity: 0.5;
        cursor: not-allowed;
        transform: none;
    }

    .nova-chatbot-typing {
        display: inline-block;
    }

    .nova-chatbot-typing-dots {
        display: inline-flex;
        align-items: center;
        gap: 4px;
    }

    .nova-chatbot-typing-dots span {
        width: 6px;
        height: 6px;
        border-radius: 50%;
        background: currentColor;
        opacity: 0.6;
        animation: typingDot 1.4s infinite ease-in-out;
    }

    .nova-chatbot-typing-dots span:nth-child(1) { animation-delay: -0.32s; }
    .nova-chatbot-typing-dots span:nth-child(2) { animation-delay: -0.16s; }

    @keyframes typingDot {
        0%, 80%, 100% { 
            transform: scale(0);
            opacity: 0.6;
        }
        40% { 
            transform: scale(1);
            opacity: 1;
        }
    }

    .nova-chatbot-body::-webkit-scrollbar {
        width: 6px;
    }

    .nova-chatbot-body::-webkit-scrollbar-track {
        background: transparent;
    }

    .nova-chatbot-body::-webkit-scrollbar-thumb {
        background: #cbd5e1;
        border-radius: 3px;
    }

    .nova-chatbot-body::-webkit-scrollbar-thumb:hover {
        background: #94a3b8;
    }

    .nova-chatbot-empty {
        text-align: center;
        padding: 2rem 1rem;
        color: #64748b;
        display: none;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        flex: 1;
    }

    .nova-chatbot-empty.show {
        display: flex;
    }

    .nova-chatbot-empty i {
        font-size: 3rem;
        margin-bottom: 1rem;
        opacity: 0.5;
    }

    .nova-chatbot-empty p {
        font-size: 0.9rem;
        margin: 0;
        text-align: center;
    }

    .nova-chatbot-listening {
        text-align: center;
        padding: 0.5rem;
        background: linear-gradient(135deg, #fed7d7 0%, #feebc8 100%);
        border-radius: 8px;
        margin: 0.5rem 0;
        animation: listeningGlow 2s infinite;
    }

    @keyframes listeningGlow {
        0%, 100% {
            box-shadow: 0 0 5px #f56565;
        }
        50% {
            box-shadow: 0 0 15px #f56565;
        }
    }

    @media (max-width: 360px) {
        .nova-chatbot-window {
            width: calc(100vw - 1rem);
            max-width: calc(100vw - 1rem);
            height: 60vh;
            max-height: 60vh;
        }

        .nova-chatbot-quick-btn {
            font-size: 0.75rem;
            padding: 0.5rem;
        }
    }

    .nova-delete-modal {
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0, 0, 0, 0.5);
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 1000000;
        opacity: 0;
        visibility: hidden;
        transition: all 0.3s ease;
    }

    .nova-delete-modal.show {
        opacity: 1;
        visibility: visible;
    }

    .nova-delete-modal-content {
        background: white;
        padding: 2rem;
        border-radius: 12px;
        max-width: 400px;
        width: 90%;
        box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
        text-align: center;
    }

    .nova-delete-modal h3 {
        margin: 0 0 1rem 0;
        color: #1e293b;
    }

    .nova-delete-modal p {
        margin: 0 0 1.5rem 0;
        color: #64748b;
        line-height: 1.5;
    }

    .nova-delete-modal-buttons {
        display: flex;
        gap: 0.75rem;
        justify-content: center;
    }

    .nova-delete-modal-btn {
        padding: 0.75rem 1.5rem;
        border: none;
        border-radius: 8px;
        font-weight: 500;
        cursor: pointer;
        transition: all 0.2s ease;
        min-width: 100px;
    }

    .nova-delete-modal-cancel {
        background: #f1f5f9;
        color: #475569;
    }

    .nova-delete-modal-cancel:hover {
        background: #e2e8f0;
    }

    .nova-delete-modal-confirm {
        background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
        color: white;
    }

    .nova-delete-modal-confirm:hover {
        background: linear-gradient(135deg, #dc2626 0%, #b91c1c 100%);
        transform: translateY(-2px);
    }
</style>

<!-- Delete Confirmation Modal -->
<div class="nova-delete-modal" id="nova-delete-modal">
    <div class="nova-delete-modal-content">
        <h3 id="nova-delete-title">Delete All Messages</h3>
        <p id="nova-delete-message">Are you sure you want to delete all chat history? This action cannot be undone.</p>
        <div class="nova-delete-modal-buttons">
            <button type="button" class="nova-delete-modal-btn nova-delete-modal-cancel" id="nova-delete-cancel">Cancel</button>
            <button type="button" class="nova-delete-modal-btn nova-delete-modal-confirm" id="nova-delete-confirm">Delete All</button>
        </div>
    </div>
</div>

<div class="nova-chatbot-window" id="nova-chatbot-window">
    <div class="nova-chatbot-header">
        <div class="nova-chatbot-header-title">
            <i class="bi bi-heart-pulse"></i>
            <div>
                <div class="nova-chatbot-title-en">Nova Assistant</div>
                <div class="nova-chatbot-title-ar" style="display:none">مساعد نوفا</div>
                <div class="nova-chatbot-status" id="nova-chatbot-status">
                    <span class="nova-chatbot-status-en">Online • AI assistant</span>
                    <span class="nova-chatbot-status-ar" style="display:none">متصل • مساعد ذكي</span>
                </div>
            </div>
        </div>
        <div class="nova-chatbot-header-controls">
            <button type="button" class="nova-chatbot-btn-icon" id="nova-chatbot-voice" title="Voice Input">
                <i class="bi bi-mic"></i>
            </button>
            <button type="button" class="nova-chatbot-btn-icon" id="nova-chatbot-history" title="View History">
                <i class="bi bi-clock-history"></i>
            </button>
            <button type="button" class="nova-chatbot-btn-icon" id="nova-chatbot-delete" title="Delete All">
                <i class="bi bi-trash3"></i>
            </button>
            <button type="button" class="nova-chatbot-btn-icon" id="nova-chatbot-close" title="Close">
                <i class="bi bi-x-lg"></i>
            </button>
        </div>
    </div>

    <div class="nova-chatbot-body" id="nova-chatbot-messages">
        <div class="nova-chatbot-empty" id="nova-chatbot-empty">
            <i class="bi bi-chat-dots"></i>
            <p class="nova-chatbot-empty-en">No messages yet. Start a conversation!</p>
            <p class="nova-chatbot-empty-ar" style="display:none">لا توجد رسائل بعد. ابدأ محادثة!</p>
        </div>
    </div>

    <div class="nova-chatbot-footer">
        <textarea 
            id="nova-chatbot-input" 
            rows="1"
            placeholder="Ask Nova a question..."
            data-en-placeholder="Ask Nova a question..."
            data-ar-placeholder="اطرح سؤالاً على نوفا..."
        ></textarea>
        <div class="nova-chatbot-footer-actions">
            <button type="button" class="nova-chatbot-btn-primary" id="nova-chatbot-send">
                <i class="bi bi-send"></i>
            </button>
        </div>
    </div>
</div>

<button type="button" class="nova-chatbot-button" id="nova-chatbot-toggle" 
    data-en-title="Chat with Nova"
    data-ar-title="التحدث مع نوفا"
    title="Chat with Nova">
    <i class="bi bi-chat-dots"></i>
</button>
`;

    document.body.appendChild(root);

    const win = document.getElementById("nova-chatbot-window");
    const toggle = document.getElementById("nova-chatbot-toggle");
    const closeBtn = document.getElementById("nova-chatbot-close");
    const messages = document.getElementById("nova-chatbot-messages");
    const emptyState = document.getElementById("nova-chatbot-empty");
    const input = document.getElementById("nova-chatbot-input");
    const sendBtn = document.getElementById("nova-chatbot-send");
    const voiceBtn = document.getElementById("nova-chatbot-voice");
    const historyBtn = document.getElementById("nova-chatbot-history");
    const deleteBtn = document.getElementById("nova-chatbot-delete");

    // Delete modal elements
    const deleteModal = document.getElementById("nova-delete-modal");
    const deleteTitle = document.getElementById("nova-delete-title");
    const deleteMessage = document.getElementById("nova-delete-message");
    const deleteCancel = document.getElementById("nova-delete-cancel");
    const deleteConfirm = document.getElementById("nova-delete-confirm");

    let isListening = false;
    let recognition = null;
    let hasWelcomeMessage = false;
    let chatHistory = JSON.parse(localStorage.getItem('nova-chat-history') || '[]');
    let currentMessages = [];

    function init() {
        updateLanguage();
        createQuickActions();
        checkEmptyState();
        loadChatHistory();
        initSpeechRecognition();
    }

    function initSpeechRecognition() {
        if ('webkitSpeechRecognition' in window || 'SpeechRecognition' in window) {
            const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
            recognition = new SpeechRecognition();
            recognition.continuous = false;
            recognition.interimResults = true;
            recognition.maxAlternatives = 1;
            recognition.lang = getCurrentLanguage() === 'ar' ? 'ar-SA' : 'en-US';

            recognition.onstart = () => {
                isListening = true;
                voiceBtn.classList.add('recording');
                voiceBtn.innerHTML = '<i class="bi bi-mic-mute"></i>';
                voiceBtn.title = translations[getCurrentLanguage()].stopListening;
                showListeningIndicator();
            };

            recognition.onresult = (event) => {
                let finalTranscript = '';
                let interimTranscript = '';

                for (let i = event.resultIndex; i < event.results.length; i++) {
                    const transcript = event.results[i][0].transcript;
                    if (event.results[i].isFinal) {
                        finalTranscript += transcript;
                    } else {
                        interimTranscript += transcript;
                    }
                }

                if (interimTranscript) {
                    input.value = interimTranscript;
                }

                if (finalTranscript) {
                    input.value = finalTranscript;
                    stopListening();
                    setTimeout(() => {
                        sendMessage();
                    }, 300);
                }
            };

            recognition.onerror = (event) => {
                console.log('Speech recognition error:', event.error);
                stopListening();
                if (event.error === 'not-allowed') {
                    alert('Microphone access is required for voice input. Please allow microphone access in your browser settings.');
                }
            };

            recognition.onend = () => {
                stopListening();
            };
        } else {
            voiceBtn.style.display = 'none';
        }
    }

    function showListeningIndicator() {
        const lang = getCurrentLanguage();
        const listeningDiv = document.createElement('div');
        listeningDiv.className = 'nova-chatbot-listening';
        listeningDiv.textContent = translations[lang].listening;
        listeningDiv.id = 'nova-chatbot-listening-indicator';

        const existing = document.getElementById('nova-chatbot-listening-indicator');
        if (existing) {
            existing.remove();
        }

        messages.appendChild(listeningDiv);
        messages.scrollTop = messages.scrollHeight;
    }

    function stopListening() {
        if (recognition && isListening) {
            recognition.stop();
        }
        isListening = false;
        voiceBtn.classList.remove('recording');
        voiceBtn.innerHTML = '<i class="bi bi-mic"></i>';
        voiceBtn.title = 'Voice Input';

        const indicator = document.getElementById('nova-chatbot-listening-indicator');
        if (indicator) {
            indicator.remove();
        }
    }

    function startListening() {
        if (!recognition) {
            alert('Speech recognition is not supported in your browser.');
            return;
        }

        try {
            recognition.start();
        } catch (err) {
            console.error('Speech recognition error:', err);
            stopListening();
        }
    }

    function toggleVoiceRecognition() {
        if (isListening) {
            stopListening();
        } else {
            startListening();
        }
    }

    function updateLanguage() {
        const lang = getCurrentLanguage();
        const isArabic = lang === 'ar';

        document.body.classList.remove('ltr', 'rtl');
        document.body.classList.add(isArabic ? 'rtl' : 'ltr');

        const titleEn = root.querySelector('.nova-chatbot-title-en');
        const titleAr = root.querySelector('.nova-chatbot-title-ar');
        if (titleEn) titleEn.style.display = isArabic ? 'none' : 'block';
        if (titleAr) titleAr.style.display = isArabic ? 'block' : 'none';

        const statusEn = root.querySelector('.nova-chatbot-status-en');
        const statusAr = root.querySelector('.nova-chatbot-status-ar');
        if (statusEn) statusEn.style.display = isArabic ? 'none' : 'inline';
        if (statusAr) statusAr.style.display = isArabic ? 'inline' : 'none';

        if (input) {
            input.placeholder = isArabic
                ? (input.dataset.arPlaceholder || '')
                : (input.dataset.enPlaceholder || '');
        }

        if (toggle) {
            toggle.title = isArabic
                ? (toggle.dataset.arTitle || '')
                : (toggle.dataset.enTitle || '');
        }

        const emptyEn = root.querySelector('.nova-chatbot-empty-en');
        const emptyAr = root.querySelector('.nova-chatbot-empty-ar');
        if (emptyEn) emptyEn.style.display = isArabic ? 'none' : 'block';
        if (emptyAr) emptyAr.style.display = isArabic ? 'block' : 'none';

        if (deleteBtn) {
            deleteBtn.title = translations[lang].deleteTitle;
        }

        if (recognition) {
            recognition.lang = isArabic ? 'ar-SA' : 'en-US';
        }

        const allMessages = messages.querySelectorAll('.nova-chatbot-message');
        allMessages.forEach(msg => msg.remove());

        // Reset welcome message flag
        hasWelcomeMessage = false;

        // Recreate quick actions
        createQuickActions();

        // Show new welcome message in current language
        setTimeout(() => {
            addWelcomeMessage();
        }, 100);
    }

    function createQuickActions() {
        const lang = getCurrentLanguage();
        const actions = quickActions[lang];

        const existing = messages.querySelector('.nova-chatbot-quick-actions');
        if (existing) {
            existing.remove();
        }

        const quickActionsDiv = document.createElement('div');
        quickActionsDiv.className = 'nova-chatbot-quick-actions';

        const title = document.createElement('div');
        title.className = 'nova-chatbot-quick-title';
        title.innerHTML = `<i class="bi bi-lightning"></i>${translations[lang].quickActions}`;

        const grid = document.createElement('div');
        grid.className = 'nova-chatbot-quick-grid';

        actions.forEach(action => {
            const button = document.createElement('button');
            button.type = 'button';
            button.className = 'nova-chatbot-quick-btn';
            button.textContent = action.text;
            button.onclick = () => {
                input.value = action.value;
                sendMessage();
            };
            grid.appendChild(button);
        });

        quickActionsDiv.appendChild(title);
        quickActionsDiv.appendChild(grid);

        const firstChild = messages.firstChild;
        if (firstChild && firstChild.id !== 'nova-chatbot-empty') {
            messages.insertBefore(quickActionsDiv, firstChild);
        } else {
            messages.appendChild(quickActionsDiv);
        }

        checkEmptyState();
    }

    function checkEmptyState() {
        const messageCount = messages.querySelectorAll('.nova-chatbot-message').length;
        const hasQuickActions = messages.querySelector('.nova-chatbot-quick-actions') !== null;

        if (messageCount === 0 && !hasWelcomeMessage) {
            emptyState.classList.add('show');
        } else {
            emptyState.classList.remove('show');
        }
    }

    function addWelcomeMessage() {
        if (hasWelcomeMessage) return;

        const lang = getCurrentLanguage();
        const welcomeMsg = translations[lang].welcome;

        const wrapper = document.createElement('div');
        wrapper.className = 'nova-chatbot-message assistant';

        const bubble = document.createElement('div');
        bubble.className = 'nova-chatbot-bubble';
        bubble.textContent = welcomeMsg;

        wrapper.appendChild(bubble);
        messages.appendChild(wrapper);

        hasWelcomeMessage = true;
        checkEmptyState();

        // Don't save welcome message to history
        if (currentMessages.length === 0) {
            saveToHistory('assistant', welcomeMsg);
        }
    }

    function setLoading(isLoading) {
        const status = document.getElementById('nova-chatbot-status');
        if (!status) return;
        const lang = getCurrentLanguage();

        if (isLoading) {
            const statusSpan = status.querySelector(`.nova-chatbot-status-${lang}`);
            if (statusSpan) {
                statusSpan.innerHTML = `<span class="nova-chatbot-typing">
                    <span class="nova-chatbot-typing-dots">
                        <span></span><span></span><span></span>
                    </span>
                </span>`;
            }
            sendBtn.disabled = true;
        } else {
            const statusSpan = status.querySelector(`.nova-chatbot-status-${lang}`);
            if (statusSpan) {
                statusSpan.textContent = translations[lang].status;
            }
            sendBtn.disabled = false;
        }
    }

    function appendMessage(text, sender) {
        if (!messages) return;

        emptyState.classList.remove('show');

        const wrapper = document.createElement('div');
        wrapper.className = `nova-chatbot-message ${sender}`;

        const bubble = document.createElement('div');
        bubble.className = 'nova-chatbot-bubble';
        bubble.textContent = text;

        wrapper.appendChild(bubble);
        messages.appendChild(wrapper);

        setTimeout(() => {
            messages.scrollTop = messages.scrollHeight;
        }, 100);

        checkEmptyState();
        saveToHistory(sender, text);
    }

    function appendMessageWithAnimation(text, sender) {
        if (!messages) return;

        emptyState.classList.remove('show');

        const wrapper = document.createElement('div');
        wrapper.className = `nova-chatbot-message ${sender}`;

        const bubble = document.createElement('div');
        bubble.className = 'nova-chatbot-bubble';

        wrapper.appendChild(bubble);
        messages.appendChild(wrapper);

        let index = 0;
        const typingSpeed = 20;

        function typeCharacter() {
            if (index < text.length) {
                bubble.textContent = text.substring(0, index + 1);
                index++;
                setTimeout(typeCharacter, typingSpeed);
            }
        }

        typeCharacter();

        setTimeout(() => {
            messages.scrollTop = messages.scrollHeight;
        }, 100);

        checkEmptyState();
        saveToHistory(sender, text);
    }

    async function sendMessage() {
        const text = input.value.trim();
        if (!text) return;

        appendMessage(text, 'user');
        input.value = '';
        setLoading(true);

        try {
            const response = await fetch('/Chat/SendMessage', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ message: text })
            });

            const data = await response.json();
            const lang = getCurrentLanguage();

            if (!response.ok || data.error) {
                const errorMsg = data.error || translations[lang].error;
                appendMessageWithAnimation(errorMsg, 'assistant');
            } else {
                const replyMsg = data.reply || translations[lang].help;
                appendMessageWithAnimation(replyMsg, 'assistant');
            }
        } catch (err) {
            const lang = getCurrentLanguage();
            const errorMsg = translations[lang].errorNetwork;
            appendMessageWithAnimation(errorMsg, 'assistant');
        } finally {
            setLoading(false);
            input.focus();
        }
    }

    function saveToHistory(sender, message) {
        if (!message.trim()) return;

        chatHistory.push({
            sender: sender,
            message: message,
            timestamp: new Date().toISOString()
        });

        if (chatHistory.length > 50) {
            chatHistory = chatHistory.slice(-50);
        }

        localStorage.setItem('nova-chat-history', JSON.stringify(chatHistory));
    }

    function loadChatHistory() {
        if (chatHistory.length === 0) return;

        chatHistory.forEach(item => {
            const wrapper = document.createElement('div');
            wrapper.className = `nova-chatbot-message ${item.sender}`;

            const bubble = document.createElement('div');
            bubble.className = 'nova-chatbot-bubble';
            bubble.textContent = item.message;

            wrapper.appendChild(bubble);
            messages.appendChild(wrapper);
        });

        hasWelcomeMessage = true;
        checkEmptyState();

        setTimeout(() => {
            messages.scrollTop = messages.scrollHeight;
        }, 100);
    }

    function showHistory() {
        if (chatHistory.length === 0) {
            alert('No chat history available.');
            return;
        }

        const lang = getCurrentLanguage();
        const historyText = chatHistory.map(item => {
            const time = new Date(item.timestamp).toLocaleTimeString();
            const sender = item.sender === 'user' ? (lang === 'ar' ? 'أنت' : 'You') : 'Nova';
            return `${time} - ${sender}: ${item.message}`;
        }).join('\n\n');

        const historyDiv = document.createElement('div');
        historyDiv.className = 'nova-chatbot-message assistant';

        const bubble = document.createElement('div');
        bubble.className = 'nova-chatbot-bubble';
        bubble.style.whiteSpace = 'pre-wrap';
        bubble.textContent = `📜 Chat History (${chatHistory.length} messages):\n\n${historyText}`;

        historyDiv.appendChild(bubble);
        messages.appendChild(historyDiv);

        setTimeout(() => {
            messages.scrollTop = messages.scrollHeight;
        }, 100);
    }

    function showDeleteConfirmation() {
        const lang = getCurrentLanguage();
        deleteTitle.textContent = translations[lang].deleteTitle;
        deleteMessage.textContent = translations[lang].deleteConfirm;
        deleteModal.classList.add('show');
    }

    function hideDeleteConfirmation() {
        deleteModal.classList.remove('show');
    }

    function deleteAllChat() {
        localStorage.removeItem('nova-chat-history');
        chatHistory = [];

        const allMessages = messages.querySelectorAll('.nova-chatbot-message');
        allMessages.forEach(msg => msg.remove());

        hasWelcomeMessage = false;
        checkEmptyState();

        setTimeout(() => {
            addWelcomeMessage();
        }, 100);

        hideDeleteConfirmation();
    }

    function clearChat() {
        const allMessages = messages.querySelectorAll('.nova-chatbot-message');
        allMessages.forEach(msg => msg.remove());

        hasWelcomeMessage = false;
        checkEmptyState();

        setTimeout(() => {
            addWelcomeMessage();
        }, 100);
    }

    toggle.addEventListener('click', (e) => {
        e.stopPropagation();
        win.classList.toggle('show');
        if (win.classList.contains('show')) {
            input.focus();
            addWelcomeMessage();
        }
    });

    closeBtn.addEventListener('click', (e) => {
        e.stopPropagation();
        win.classList.remove('show');
    });

    sendBtn.addEventListener('click', sendMessage);

    input.addEventListener('keydown', (e) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            sendMessage();
        }

        setTimeout(() => {
            input.style.height = 'auto';
            input.style.height = Math.min(input.scrollHeight, 120) + 'px';
        }, 0);
    });

    voiceBtn.addEventListener('click', (e) => {
        e.stopPropagation();
        toggleVoiceRecognition();
    });

    historyBtn.addEventListener('click', (e) => {
        e.stopPropagation();
        showHistory();
    });

    deleteBtn.addEventListener('click', (e) => {
        e.stopPropagation();
        showDeleteConfirmation();
    });

    deleteCancel.addEventListener('click', hideDeleteConfirmation);
    deleteConfirm.addEventListener('click', deleteAllChat);

    deleteModal.addEventListener('click', (e) => {
        if (e.target === deleteModal) {
            hideDeleteConfirmation();
        }
    });

    document.addEventListener('click', (e) => {
        if (!win.contains(e.target) && !toggle.contains(e.target) && !deleteModal.contains(e.target)) {
            win.classList.remove('show');
        }
    });

    win.addEventListener('click', (e) => {
        e.stopPropagation();
    });

    document.addEventListener('languageChanged', () => {
        updateLanguage();
    });

    document.addEventListener('clearChat', () => {
        clearChat();
    });

    init();

    win.addEventListener('transitionend', () => {
        if (win.classList.contains('show')) {
            input.focus();
        }
    });
})();