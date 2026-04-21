// FAQ Data
const faqData = [
    {
        title: 'Ordering',
        faqs: [
            {
                question: 'Do you ship worldwide?',
                answer: 'Unfortunately, we currently do not ship worldwide. We only deliver within Vietnam from our warehouse in HCM city.'
            },
            {
                question: 'How long does shipping take?',
                answer: 'Standard shipping within Vietnam typically takes 2-5 business days depending on your location. Express shipping options are available for faster delivery.'
            },
            {
                question: 'Can I track my order?',
                answer: 'Yes! Once your order ships, you will receive a tracking number via email. You can use this to monitor your delivery status in real-time.'
            },
            {
                question: 'What are the shipping costs?',
                answer: 'Shipping costs vary based on your location and chosen delivery method. Free shipping is available on orders over 500,000 VND within Vietnam.'
            }
        ]
    },
    {
        title: 'Product',
        faqs: [
            {
                question: 'What materials are your eyewear made from?',
                answer: 'Our eyewear is crafted from premium, sustainable materials including carbon-negative acetate, recycled metals, and eco-friendly plastics. Each piece is designed with both style and environmental responsibility in mind.'
            },
            {
                question: 'Do you offer prescription lenses?',
                answer: 'Yes, we offer prescription lenses for all our frames. Simply provide your prescription details during checkout, and our optical experts will prepare your custom lenses.'
            },
            {
                question: 'Are the frames adjustable?',
                answer: 'Most of our frames can be adjusted for a better fit. We recommend visiting an optician for professional adjustments, or you can follow our online fitting guide.'
            },
            {
                question: 'What is your sustainability commitment?',
                answer: 'We are committed to carbon-negative production, using eco-friendly materials, and implementing sustainable practices throughout our supply chain. Every purchase contributes to environmental conservation efforts.'
            }
        ]
    },
    {
        title: 'Returns',
        faqs: [
            {
                question: 'What is your return policy?',
                answer: 'We offer a 30-day return policy for all unworn, undamaged eyewear in original packaging. If you are not completely satisfied with your purchase, you can return it for a full refund or exchange.'
            },
            {
                question: 'How do I initiate a return?',
                answer: 'To start a return, contact our customer service team with your order number. We will provide you with return instructions and a prepaid shipping label.'
            },
            {
                question: 'Are prescription lenses returnable?',
                answer: 'Due to the custom nature of prescription lenses, they are non-returnable unless there is a manufacturing defect or error in fulfilling your prescription.'
            },
            {
                question: 'When will I receive my refund?',
                answer: 'Once we receive and inspect your return, refunds are processed within 5-7 business days. The refund will be credited to your original payment method.'
            }
        ]
    },
    {
        title: 'Payment',
        faqs: [
            {
                question: 'What payment methods do you accept?',
                answer: 'We accept major credit cards (Visa, Mastercard, American Express), debit cards, PayPal, and local Vietnamese payment methods including bank transfers and cash on delivery.'
            },
            {
                question: 'Is my payment information secure?',
                answer: 'Absolutely. We use industry-standard SSL encryption to protect your payment information. All transactions are processed through secure payment gateways that comply with PCI DSS standards.'
            },
            {
                question: 'Do you offer installment payments?',
                answer: 'Yes, we partner with several payment providers to offer installment payment options. You can choose to split your payment into 3-6 monthly installments at checkout.'
            },
            {
                question: 'Can I use multiple payment methods for one order?',
                answer: 'Currently, we only accept one payment method per order. However, you can use gift cards or promotional codes in combination with your primary payment method.'
            }
        ]
    }
];

// SVG Arrow Icon
const arrowIcon = `<svg class="arrow-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
</svg>`;

// Create FAQ HTML
function createFAQHTML() {
    const container = document.getElementById('faq-container');
    
    faqData.forEach((section, sectionIndex) => {
        const sectionDiv = document.createElement('div');
        sectionDiv.className = 'faq-section';
        
        let sectionHTML = `
            <h2 class="section-title">${section.title}</h2>
            <div class="faq-list">
        `;
        
        section.faqs.forEach((faq, faqIndex) => {
            const uniqueId = `faq-${sectionIndex}-${faqIndex}`;
            sectionHTML += `
                <div class="faq-item">
                    <button class="faq-question" data-id="${uniqueId}">
                        <span class="question-text">${faq.question}</span>
                        ${arrowIcon}
                    </button>
                    <div class="faq-answer" id="${uniqueId}">
                        <p class="answer-text">${faq.answer}</p>
                    </div>
                </div>
            `;
        });
        
        sectionHTML += `</div>`;
        sectionDiv.innerHTML = sectionHTML;
        container.appendChild(sectionDiv);
    });
    
    // Add event listeners
    addEventListeners();
}

// Toggle FAQ Answer
function toggleFAQ(button) {
    const answerId = button.getAttribute('data-id');
    const answer = document.getElementById(answerId);
    const arrow = button.querySelector('.arrow-icon');
    
    // Close all other open FAQs in the same section
    const allButtons = button.closest('.faq-list').querySelectorAll('.faq-question');
    allButtons.forEach(btn => {
        if (btn !== button) {
            const otherAnswerId = btn.getAttribute('data-id');
            const otherAnswer = document.getElementById(otherAnswerId);
            const otherArrow = btn.querySelector('.arrow-icon');
            otherAnswer.classList.remove('open');
            otherArrow.classList.remove('open');
        }
    });
    
    // Toggle current FAQ
    answer.classList.toggle('open');
    arrow.classList.toggle('open');
}

// Add Event Listeners
function addEventListeners() {
    const questions = document.querySelectorAll('.faq-question');
    questions.forEach(question => {
        question.addEventListener('click', function() {
            toggleFAQ(this);
        });
    });
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', createFAQHTML);