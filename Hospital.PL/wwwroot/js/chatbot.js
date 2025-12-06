// Simple floating chatbot widget for NovaHealth
(function () {
    // Get current language
    function getCurrentLanguage() {
        return localStorage.getItem("language") === "ar" ? "ar" : "en";
    }

    // Translations
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
            buttonTitle: "Chat with Nova"
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
            buttonTitle: "التحدث مع نوفا"
        }
    };

    const root = document.createElement("div");
    root.id = "nova-chatbot-root";
    root.innerHTML = `
<style>
    #nova-chatbot-root {
        position: fixed;
        /* Position AI button above dark-mode-toggle button (which is at bottom: 2rem) */
        bottom: calc(2rem + 50px + 10px);
        right: 2rem;
        z-index: 1050;
        font-family: "Poppins", system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif;
    }
    body.rtl #nova-chatbot-root {
        right: auto;
        left: 2rem;
    }
    .nova-chatbot-button {
        width: 50px;
        height: 50px;
        border-radius: 50%;
        border: none;
        background: linear-gradient(135deg, #0d6efd, #6610f2);
        color: #fff;
        display: flex;
        align-items: center;
        justify-content: center;
        box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
        cursor: pointer;
        transition: transform 0.3s ease;
    }
    .nova-chatbot-button:hover {
        transform: scale(1.1);
    }
    .nova-chatbot-button i {
        font-size: 1.25rem;
    }
    .nova-chatbot-window {
        position: absolute;
        bottom: 0;
        right: 70px;
        width: 1000px;
        max-width: calc(100vw - 140px);
        height: 85vh;
        max-height: 85vh;
        min-height: 600px;
        background: #ffffff;
        border-radius: 16px;
        box-shadow: 0 18px 45px rgba(15, 23, 42, 0.35);
        display: flex;
        flex-direction: column;
        overflow: hidden;
        direction: ltr;
    }
    body.rtl .nova-chatbot-window {
        right: auto;
        left: 70px;
        direction: rtl;
    }
    .nova-chatbot-header {
        padding: 0.75rem 1rem;
        background: linear-gradient(135deg, #0d6efd, #20c997);
        color: #fff;
        display: flex;
        align-items: center;
        justify-content: space-between;
    }
    .nova-chatbot-header-title {
        display: flex;
        align-items: center;
        gap: 0.5rem;
        font-weight: 600;
    }
    .nova-chatbot-header-title i {
        font-size: 1.25rem;
    }
    .nova-chatbot-status {
        font-size: 0.75rem;
        opacity: 0.85;
    }
    .nova-chatbot-body {
        padding: 0.75rem;
        background: #f8fafc;
        overflow-y: auto;
        flex: 1;
    }
    .nova-chatbot-message {
        margin-bottom: 0.5rem;
        display: flex;
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
        max-width: 80%;
        padding: 0.5rem 0.75rem;
        border-radius: 14px;
        font-size: 0.85rem;
        line-height: 1.5;
        word-wrap: break-word;
        text-align: left;
    }
    body.rtl .nova-chatbot-bubble {
        text-align: right;
    }
    .nova-chatbot-message.user .nova-chatbot-bubble {
        background: #0d6efd;
        color: #fff;
        border-bottom-right-radius: 2px;
        border-bottom-left-radius: 14px;
    }
    body.rtl .nova-chatbot-message.user .nova-chatbot-bubble {
        border-bottom-right-radius: 14px;
        border-bottom-left-radius: 2px;
    }
    .nova-chatbot-message.assistant .nova-chatbot-bubble {
        background: #e2f0ff;
        color: #0f172a;
        border-bottom-left-radius: 2px;
        border-bottom-right-radius: 14px;
    }
    body.rtl .nova-chatbot-message.assistant .nova-chatbot-bubble {
        border-bottom-left-radius: 14px;
        border-bottom-right-radius: 2px;
    }
    .nova-chatbot-footer {
        padding: 0.75rem;
        border-top: 1px solid #e5e7eb;
        background: #ffffff;
        display: flex;
        gap: 0.6rem;
        align-items: center;
        flex-direction: row;
    }
    body.rtl .nova-chatbot-footer {
        flex-direction: row-reverse;
    }
    .nova-chatbot-footer textarea {
        flex: 1;
        min-width: 200px;
        resize: none;
        border-radius: 999px;
        border: 1px solid #d1d5db;
        padding: 0.75rem 1rem;
        font-size: 1rem;
        max-height: 100px;
        min-height: 44px;
        order: 1;
        direction: ltr;
        text-align: left;
    }
    body.rtl .nova-chatbot-footer textarea {
        direction: rtl;
        text-align: right;
        order: 2;
    }
    .nova-chatbot-send-btn {
        border-radius: 999px;
        border: none;
        padding: 0.75rem 1.2rem;
        background: #0d6efd;
        color: #fff;
        display: flex;
        align-items: center;
        justify-content: center;
        cursor: pointer;
        min-width: 50px;
        height: 44px;
        order: 2;
        flex-shrink: 0;
    }
    body.rtl .nova-chatbot-send-btn {
        order: 1;
    }
    .nova-chatbot-send-btn i {
        font-size: 1.1rem;
    }
    .nova-chatbot-send-btn:disabled {
        opacity: 0.6;
        cursor: not-allowed;
    }
    
    /* Responsive styles */
    @media (max-width: 768px) {
        .nova-chatbot-window {
            width: calc(100vw - 20px);
            max-width: calc(100vw - 20px);
            right: 10px;
            left: 10px;
            bottom: calc(2rem + 50px + 10px);
            height: calc(100vh - 150px);
            min-height: 400px;
        }
        body.rtl .nova-chatbot-window {
            right: 10px;
            left: 10px;
        }
        #nova-chatbot-root {
            bottom: calc(2rem + 50px + 10px);
            right: 1rem;
        }
        body.rtl #nova-chatbot-root {
            left: 1rem;
        }
    }
    
    @media (max-width: 480px) {
        .nova-chatbot-window {
            width: calc(100vw - 10px);
            max-width: calc(100vw - 10px);
            right: 5px;
            left: 5px;
            height: calc(100vh - 120px);
            min-height: 350px;
        }
        body.rtl .nova-chatbot-window {
            right: 5px;
            left: 5px;
        }
        .nova-chatbot-footer textarea {
            min-width: 150px;
            font-size: 0.9rem;
        }
    }
</style>
<div class="nova-chatbot-window d-none" id="nova-chatbot-window">
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
        <button type="button" class="btn btn-sm btn-light" id="nova-chatbot-close">&times;</button>
    </div>
    <div class="nova-chatbot-body" id="nova-chatbot-messages">
        <div class="nova-chatbot-message assistant">
            <div class="nova-chatbot-bubble">
                <span class="nova-chatbot-welcome-en">Hi, I'm Nova, your AI assistant. I can help with NovaHealth departments, doctors, appointments, and general questions.</span>
                <span class="nova-chatbot-welcome-ar" style="display:none">مرحباً، أنا نوفا، مساعدك الذكي. يمكنني المساعدة في أقسام نوفا هيلث والأطباء والمواعيد والأسئلة العامة.</span>
            </div>
        </div>
    </div>
    <div class="nova-chatbot-footer">
        <textarea id="nova-chatbot-input" rows="1" 
            data-en-placeholder="Ask Nova a question..."
            data-ar-placeholder="اطرح سؤالاً على نوفا..."
            placeholder="Ask Nova a question..."></textarea>
        <button type="button" class="nova-chatbot-send-btn" id="nova-chatbot-send">
            <i class="bi bi-send"></i>
        </button>
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
    const input = document.getElementById("nova-chatbot-input");
    const sendBtn = document.getElementById("nova-chatbot-send");
    const status = document.getElementById("nova-chatbot-status");

    // Update UI based on language
    function updateLanguage() {
        const lang = getCurrentLanguage();
        const isArabic = lang === "ar";
        
        // Update title
        const titleEn = root.querySelector(".nova-chatbot-title-en");
        const titleAr = root.querySelector(".nova-chatbot-title-ar");
        if (titleEn) titleEn.style.display = isArabic ? "none" : "block";
        if (titleAr) titleAr.style.display = isArabic ? "block" : "none";
        
        // Update status
        const statusEn = root.querySelector(".nova-chatbot-status-en");
        const statusAr = root.querySelector(".nova-chatbot-status-ar");
        if (statusEn) statusEn.style.display = isArabic ? "none" : "inline";
        if (statusAr) statusAr.style.display = isArabic ? "inline" : "none";
        
        // Update welcome message
        const welcomeEn = root.querySelector(".nova-chatbot-welcome-en");
        const welcomeAr = root.querySelector(".nova-chatbot-welcome-ar");
        if (welcomeEn) welcomeEn.style.display = isArabic ? "none" : "inline";
        if (welcomeAr) welcomeAr.style.display = isArabic ? "inline" : "none";
        
        // Update placeholder using the same method as index.js
        if (input) {
            if (!input.dataset.enPlaceholder) {
                input.dataset.enPlaceholder = input.getAttribute("placeholder") || "";
            }
            input.placeholder = isArabic 
                ? (input.dataset.arPlaceholder || "") 
                : (input.dataset.enPlaceholder || "");
        }
        
        // Update button title
        if (toggle) {
            toggle.title = isArabic 
                ? (toggle.dataset.arTitle || "") 
                : (toggle.dataset.enTitle || "");
        }
    }

    function setLoading(isLoading) {
        if (!status) return;
        const lang = getCurrentLanguage();
        const isArabic = lang === "ar";
        
        const statusEn = root.querySelector(".nova-chatbot-status-en");
        const statusAr = root.querySelector(".nova-chatbot-status-ar");
        
        if (isLoading) {
            if (statusEn) statusEn.textContent = isArabic ? "جاري التفكير..." : "Thinking...";
            if (statusAr) statusAr.textContent = "جاري التفكير...";
        } else {
            if (statusEn) statusEn.textContent = isArabic ? "متصل • مساعد ذكي" : "Online • AI assistant";
            if (statusAr) statusAr.textContent = "متصل • مساعد ذكي";
        }
    }

    function appendMessage(text, sender) {
        if (!messages) return;
        const wrapper = document.createElement("div");
        wrapper.className = "nova-chatbot-message " + sender;
        const bubble = document.createElement("div");
        bubble.className = "nova-chatbot-bubble";
        bubble.textContent = text;
        wrapper.appendChild(bubble);
        messages.appendChild(wrapper);
        messages.scrollTop = messages.scrollHeight;
    }

    async function sendMessage() {
        const text = input.value.trim();
        if (!text) return;

        appendMessage(text, "user");
        input.value = "";
        sendBtn.disabled = true;
        setLoading(true);

        try {
            const response = await fetch("/Chat/SendMessage", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ message: text })
            });

            const data = await response.json();
            const lang = getCurrentLanguage();
            const isArabic = lang === "ar";
            
            if (!response.ok || data.error) {
                const errorMsg = data.error || (isArabic ? translations.ar.error : translations.en.error);
                appendMessage(errorMsg, "assistant");
            } else {
                const replyMsg = data.reply || (isArabic ? translations.ar.help : translations.en.help);
                appendMessage(replyMsg, "assistant");
            }
        } catch (err) {
            const lang = getCurrentLanguage();
            const isArabic = lang === "ar";
            const errorMsg = isArabic ? translations.ar.errorNetwork : translations.en.errorNetwork;
            appendMessage(errorMsg, "assistant");
        } finally {
            sendBtn.disabled = false;
            setLoading(false);
            input.focus();
        }
    }

    toggle.addEventListener("click", function () {
        if (win.classList.contains("d-none")) {
            win.classList.remove("d-none");
        } else {
            win.classList.add("d-none");
        }
    });

    closeBtn.addEventListener("click", function () {
        win.classList.add("d-none");
    });

    sendBtn.addEventListener("click", function () {
        sendMessage();
    });

    input.addEventListener("keydown", function (e) {
        if (e.key === "Enter" && !e.shiftKey) {
            e.preventDefault();
            sendMessage();
        }
    });

    // Listen for language changes
    document.addEventListener("languageChanged", function() {
        updateLanguage();
    });

    // Initialize language on load
    updateLanguage();
})();


