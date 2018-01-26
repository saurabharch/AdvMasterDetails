var cacheName = "PromaxCaches";
var cacheFiles = [
    './',
    'https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js',
    './matrial_content/Style.css',
        'https://fonts.googleapis.com/icon?family=Material+Icons',
           './matrial_content/material.min.js',
            './matrial_content/main.css',
            './matrial_content/app.js',
            './View/Push/index.html'
]

self.addEventListener('install', function (e) {
    console.log("[ServiceWorker] Insatlled");

    e.waitUntil(
        caches.open(cacheName).then(function (cache) {
            console.log("[ServiceWorker] Caching CacheFiles");
            return cache.addAll(cacheFiles);
        })
    )
})

self.addEventListener('activate', function (e) {
    console.log("[ServiceWorker] Activated");
    e.waitUntil(

        // Get all the cache keys (cacheName)
        caches.keys().then(function (cacheNames) {
            return Promise.all(cacheNames.map(function (thisCacheName) {

                // If a cached item is saved under a previous cacheName
                if (thisCacheName !== cacheName) {

                    // Delete that cached file
                    console.log('[ServiceWorker] Removing Cached Files from Cache - ', thisCacheName);
                    return caches.delete(thisCacheName);
                }
            }));
        })
    ); // end e.waitUntil
})

self.addEventListener('fetch', function (e) {
    console.log("[ServiceWorker] fetching");
    // e.respondWidth Responds to the fetch event
    e.respondWith(

        // Check in cache for the request being made
        caches.match(e.request)


            .then(function (response) {

                // If the request is in the cache
                if (response) {
                    console.log("[ServiceWorker] Found in Cache", e.request.url, response);
                    // Return the cached version
                    return response;
                }

                // If the request is NOT in the cache, fetch and cache

                var requestClone = e.request.clone();
                fetch(requestClone)
                    .then(function (response) {

                        if (!response) {
                            console.log("[ServiceWorker] No response from fetch ");
                            return response;
                        }

                        var responseClone = response.clone();

                        //  Open the cache
                        caches.open(cacheName).then(function (cache) {

                            // Put the fetched response in the cache
                            cache.put(e.request, responseClone);
                            console.log('[ServiceWorker] New Data Cached', e.request.url);

                            // Return the response
                            return response;

                        }); // end caches.open

                    })
                    .catch(function (err) {
                        console.log('[ServiceWorker] Error Fetching & Caching New Data', err);
                    });


            }) // end caches.match(e.request)
    ); // end e.respondWith
});
