$(document).ready(function () {
    function showToast(title, message, type = 'success') {
        // if toast container doesn't exist, create it
        let toastContainer = $('.toast-container');
        if (toastContainer.length === 0) {
            $('body').append('<div class="toast-container position-fixed bottom-0 end-0 p-3" style="z-index: 1100"></div>');
            toastContainer = $('.toast-container');
        }

       
        const colors = {
            success: {
                bg: 'rgba(42, 95, 58, 0.85)',
                border: '#4caf50',
                icon: 'bi-check-circle-fill',
                glow: 'rgba(76, 175, 80, 0.4)'
            },
            error: {
                bg: 'rgba(95, 42, 42, 0.85)',
                border: '#dc3545',
                icon: 'bi-exclamation-triangle-fill',
                glow: 'rgba(220, 53, 69, 0.4)'
            },
            warning: {
                bg: 'rgba(95, 74, 42, 0.85)',
                border: '#ffc107',
                icon: 'bi-exclamation-triangle-fill',
                glow: 'rgba(255, 193, 7, 0.4)'
            },
            info: {
                bg: 'rgba(42, 74, 95, 0.85)',
                border: '#17a2b8',
                icon: 'bi-info-circle-fill',
                glow: 'rgba(23, 162, 184, 0.4)'
            }
        };

        const color = colors[type] || colors.success;

        const toastHtml = `
            <div class="toast align-items-center text-white border-0 show mb-2" role="alert" 
                 style="
                    background: ${color.bg};
                    backdrop-filter: blur(20px);
                    -webkit-backdrop-filter: blur(20px);
                    border-left: 4px solid ${color.border};
                    border-radius: 14px;
                    min-width: 320px;
                    box-shadow: 0 20px 60px rgba(0,0,0,0.5), 0 0 30px ${color.glow};
                    transition: all 0.3s ease;
                 ">
                <div class="toast-header" 
                     style="
                        background: transparent;
                        color: #ffd6b0;
                        border-bottom: 1px solid rgba(192, 154, 108, 0.2);
                        padding: 12px 16px;
                        border-radius: 14px 14px 0 0;
                     ">
                    <i class="bi ${color.icon} me-2" style="color: ${color.border}; font-size: 1.2rem;"></i>
                    <strong class="me-auto" style="font-weight: 600; letter-spacing: 0.3px;">${title}</strong>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" 
                            style="opacity: 0.7; filter: drop-shadow(0 0 4px rgba(0,0,0,0.3));"></button>
                </div>
                <div class="toast-body" 
                     style="
                        color: #e8d9cc;
                        padding: 14px 16px;
                        font-size: 0.95rem;
                        border-radius: 0 0 14px 14px;
                        background: rgba(0,0,0,0.1);
                     ">
                    <i class="bi bi-egg-fried me-2" style="color: ${color.border};"></i>
                    ${message}
                </div>
            </div>
        `;

        const toastElement = $(toastHtml);
        toastContainer.append(toastElement);

       
        toastElement.css('opacity', '0').animate({ opacity: '1' }, 300);

        // auto-hide after 5 seconds
        setTimeout(() => {
            toastElement.fadeOut(500, function () {
                $(this).remove();
            });
        }, 5000);
    }

    // expose to global scope
    window.showToast = showToast;

    // read temp data from hidden inputs
    const success = $('#tempDataSuccess').val();
    const error = $('#tempDataError').val();
    const warning = $('#tempDataWarning').val();
    const info = $('#tempDataInfo').val();

    if (success && success !== '' && success !== 'null') {
        showToast('✨ Success!', success, 'success');
        $('#tempDataSuccess').val('');
    }
    if (error && error !== '' && error !== 'null') {
        showToast('❌ Error!', error, 'error');
        $('#tempDataError').val('');
    }
    if (warning && warning !== '' && warning !== 'null') {
        showToast('⚠️ Warning', warning, 'warning');
        $('#tempDataWarning').val('');
    }
    if (info && info !== '' && info !== 'null') {
        showToast('ℹ️ Info', info, 'info');
        $('#tempDataInfo').val('');
    }

    // global AJAX listener
    $(document).ajaxComplete(function (event, xhr, settings) {
        const toast = xhr.getResponseHeader('X-Toast-Message');
        const toastType = xhr.getResponseHeader('X-Toast-Type') || 'success';

        if (toast) {
            showToast('🏺 Furniture & Garden', toast, toastType);
        }
    });
});