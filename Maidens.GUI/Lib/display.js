var timeouts = [];

function init() {
    pageScroll();
    timeouts.push(setTimeout('pageScroll()', 500));
}



function pageScroll() {
    window.scrollBy(0, 10);

    var yLeftToGo = document.height - (window.pageYOffset + window.innerHeight);

    if (yLeftToGo == 0) {
        for (var i = 0; i < timeouts.length; i++) {
            clearTimeout(timeouts[i]);
        }
    }
    else {
        timeouts.push(setTimeout('pageScroll()', 500));
    }
}

window.onload = init;