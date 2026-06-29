function handleImageError(img) {
    console.log('Image failed to load:', img.src);
    img.style.display = 'none';

    const parent = img.parentElement;
    const fallbackDiv = document.createElement('div');
    fallbackDiv.className = 'webpage-card glass-card p-4';
    fallbackDiv.innerHTML = `
        <i class="bi bi-exclamation-triangle" style="font-size: 3rem; color: #ffc107;"></i>
        <p class="mt-3">Image could not be loaded</p>
        <a href="${img.src}" target="_blank" class="btn btn-outline-glass">
            View Original Link
        </a>
    `;
    parent.appendChild(fallbackDiv);
}

async function fetchPagePreview(url) {
    const container = document.getElementById('previewContainer');
    const img = document.getElementById('fetchedImage');

    container.style.display = 'block';
    img.src = '';

    img.insertAdjacentHTML('beforebegin', '<div class="spinner-border text-warning" id="loadingSpinner"></div>');

    try {
        const proxyUrl = `https://api.microlink.io/?url=${encodeURIComponent(url)}&screenshot=true&meta=false`;
        const response = await fetch(proxyUrl);
        const data = await response.json();
        document.getElementById('loadingSpinner')?.remove();

        if (data.data?.image?.url) {
            img.src = data.data.image.url;
        } else if (data.data?.screenshot?.url) {
            img.src = data.data.screenshot.url;
        } else {
            throw new Error('No image found');
        }
    } catch (error) {
        document.getElementById('loadingSpinner')?.remove();
        container.innerHTML = `
            <div class="alert alert-warning">
                Could not fetch page preview.
                <a href="${url}" target="_blank">Visit page directly</a>
            </div>
        `;
    }
}