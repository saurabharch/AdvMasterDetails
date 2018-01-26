if ('serviceWorker' in navigator) {
    navigator.serviceWorker
    .register('http://localhost:47367/matrial_content/service-worker.js', { scope: '/' })
    .then(function (registration) {
        console.log("Service Worker Registered");
    }).catch(function (err) {
        console.log("Service Worker Failed to Registered", err);
    });
}

// Function to perform HTTP request
/* var get = function (url) {
    return new Promise(function (resolve, reject) {

        var xhr = new XMLHttpRequest();
        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                if (xhr.status === 200) {
                    var result = xhr.responseText
                    result = JSON.parse(result);
                    resolve(result);
                } else {
                    reject(xhr);
                }
            }
        };

        xhr.open("GET", url, true);
        xhr.send();

    });
};


get('https://api.nasa.gov/planetary/earth/imagery?api_key=fWfSMcDzyHfMuH3BW6jiIUBYaj3hKRyKBRTBqgEQ')
    .then(function (response) {
        // There is an issue with the image being pulled from the API, so using a different one instead
        document.getElementsByClassName('targetImage')[0].src = "https://api.nasa.gov/images/earth.png";

    })
    .catch(function (err) {
        console.log("Error", err);
/*     }); */
/* var popup = function () {
    var news = "data.news"; //Client Guid or ID asssigne for Browser
    var title = "data.title"; // Tittle
    var bodymsg = "data.bodymsg";//Message
    var image = "data.img"; // Logo Image Link
    var url = "data.url";//Page Url Redirect Link
    var tagsid = "data.TagID";
    var Status = "data.news";
    if ("Notification" in window) {
        let ask = Notification.requestPermission();
        ask.then(permission => {
            if (permission === "granted") {
                if (news === tagsid && data.Status) {
                    let msg = new Notification(title, {
                        body: bodymsg,
                        icon: image,
                        tag: tagsid
                    });
                    msg.onshow = function () {
                        setTimeout(msg.close, 15000)
                    },
                        msg.addEventListener('click', event => {
                            window.location.replace(url);
                        })
                }
            }
        })
    }
};
popup(); */
var popup = function () {
    var news = 'Hello World'; //Client Guid or ID asssigne for Browser
    var title = 'Hello World'; // Tittle
    var bodymsg = 'Hello World';//Message
    var image = ''; // Logo Image Link
    var url = 'http://localhost:5500/index.html';//Page Url Redirect Link
    var tagsid = 'Hello World';
    var Status = 'Hello World';
    if ("Notification" in window) {
        let ask = Notification.requestPermission();
        ask.then(permission => {
            if (permission === "granted") {
                if (news === tagsid) {
                    let msg = new Notification(title, {
                        body: bodymsg,
                        icon: image,
                        tag: tagsid
                    });
                    msg.onshow = function () {
                        setTimeout(msg.close, 15000)
                    }
                    msg.addEventListener('click', event => {
                        window.location.replace(url);
                    })
                }
            }
        });
    }
};

popup();