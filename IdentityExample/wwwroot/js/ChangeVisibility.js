////$(document).ready(() => {
////    function showMore() {
////        $(this).removeClass('shown');
////        $('.long-desc').addClass('shown');
////        console.log("Show more");
////        console.log($(this));
////    }
////    function showLess() {
////        $('.long-desc').removeClass('shown');
////        $('#btn-desc-showmore').addClass('shown');
////        console.log("Show less");
////        console.log($(this));
////    }
////    $('#btn-desc-showmore').click(function () {
       
////    });
////    $('#btn-desc-showsmall').click(function () {
////        //$('.long-desc').removeClass('shown');
////        //$('#btn-desc-showmore').addClass('shown');
////        //console.log("Show less");
////        //console.log($(this));
////    });
////})


function showReplies(e) {
    let allReplies = document.getElementById(e+"-all-replies");
    let hideButton = document.getElementById(e+"-hide-comment-replies");
    let showButton = document.getElementById(e+"-show-comment-replies");
    hideButton.toggleAttribute("hidden");
    showButton.toggleAttribute("hidden");
    allReplies.toggleAttribute("hidden");
}

function replyIt(e) {
    document.getElementById(e+"-comment-reply-form").toggleAttribute("hidden");
}





function processVisibilityChanges() {
    let showMoreAnch = document.getElementById("btn-desc-showmore");
    showMoreAnch.classList.toggle("shown");
    longDesc = document.querySelector(".long-desc");
    longDesc.classList.toggle("shown");
}
document.addEventListener("load", () => {
    let showMoreAnch = document.getElementById("btn-desc-showmore");
    let showSmallAnch = document.getElementById("btn-desc-showsmall");
    let login = document.getElementById("loginButton");
    showMoreAnch.addEventListener("click", () => {
        processVisibilityChanges();
    });
    showSmallAnch.addEventListener("click", () => {
        processVisibilityChanges();
    });
    login.addEventListener("click", () => {
        console.log('qwer');
        formLogin();
    })
});


async function formLogin() {
    console.log('qqq');
    const resp = await fetch("/admin/Account/Login", { method: "GET" });
    if (resp.ok === true) {
        let formLogin = document.getElementById("loginWindow");
        let htmlLogin = await resp.text();
        formLogin.innerHTML = htmlLogin;
    }
}


