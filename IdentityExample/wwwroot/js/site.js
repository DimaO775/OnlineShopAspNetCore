//const { contains } = require("jquery");

$('.dropdown1').click(function () {
    $(this).attr('tabindex', 1).focus();
    $(this).toggleClass('active');
    $(this).find('.dropdown-menu1').slideToggle(300);    
});
$('.dropdown1 .dropdown-menu1').focusout(function () {
    $(this).toggleClass('active')
    $(this).find('.dropdown-menu1').slideToggle(300);
    
});


function loginForm() {
    if (document.querySelector('.register-window').hasAttribute('hidden')) {
        document.getElementById("modalWrapper").classList.toggle("modal-wrapper");
        document.getElementById("overlay").classList.toggle("overlay");
        document.querySelector('.login-window').toggleAttribute("hidden");
    }
}

function registerForm() {
    loginForm();
    document.getElementById("modalWrapper1").classList.toggle("modal-wrapper");
    document.getElementById("overlay1").classList.toggle("overlay");
    document.querySelector('.register-window').toggleAttribute("hidden");
    
}


document.getElementById("loginButton").addEventListener("click", loginForm);
document.getElementById("close-login-window").addEventListener("click", loginForm);
document.getElementById("loginButton").addEventListener("focusout", loginForm);

document.getElementById("register").addEventListener("click", registerForm)


async function loginAsync(e) {
    e.preventDefault();
    const formData = new FormData();
    const loginForm = document.forms.login;
    let returnUrl = loginForm.elements[0].value;
    let loginName = loginForm.elements[1].value;
    let passwordName = loginForm.elements[2].value;
    let isPersistent = loginForm.elements[3].value;
    formData.append("returnUrl", returnUrl);
    formData.append("Login", loginName);
    formData.append("Password", passwordName);
    formData.append("IsPersistent", isPersistent);
    const resp = await fetch("/admin/Account/Login", { method: "Post", body: formData });
    if (resp.ok === true) {
        let html = await resp.text();
        let loginFormWrapper = document.getElementById('loginFormWrapper');
        loginFormWrapper.innerHTML = html;
    }
}
function changePrice() {
/*    var minPrice = Number(document.getElementById('minPriceRange').value);
    var maxPrice = Number(document.getElementById('maxPriceRange').value);*/

    /*var minPriceRange = document.querySelector('.noUi-handle-upper').getAttribute('aria-valuemin');
    var maxPriceRange = document.querySelector('.noUi-handle-upper').getAttribute('aria-valuenow');
    var minPriceText = document.getElementById('minPriceText');
    var maxPriceText = document.getElementById('maxPriceText');

    minPriceText.value = Number(minPriceRange);
    maxPriceText.value = Number(maxPriceRange);*/

   

}
async function paymentCard() {
    const resp = await fetch("/admin/Order/PaymentCard");
    if (resp.ok === true) {
        let html = await resp.text();
        let paymentMethod = document.getElementById('payment-method');
        paymentMethod.innerHTML = html;
    }
}
async function paymentCash() {
    let html = '';
    let paymentMethod = document.getElementById('payment-method');
    paymentMethod.innerHTML = html;
}
async function paymentPayPal() {
    let html = '';
    let paymentMethod = document.getElementById('payment-method');
    paymentMethod.innerHTML = html;
}

function changeQuantity(id) {  
    let cartNotify = document.getElementById("cartNotify");
    let idElem = "quantityPr" + id;
    let IdButton = "IdButton" + id;
    let inputQuantity = document.getElementById(idElem);

   // let productInCartQuantity = Number(inputQuantity.getAttribute("productInCartQuantity"));
    let productQuantity = Number(inputQuantity.getAttribute("productQuantity"));
    let productTitle = inputQuantity.getAttribute("productTitle");

    let html = `
            <div style="width: 500px; background-color: #d96868; border-radius:5px; margin-left:30%; padding: 10px; opacity: 0.8">
                <p style="margin:0">Товара ${productTitle} нет в количестве ${inputQuantity.value} на складе!</p>
            </div>
            `;
    //let changed = Number(inputQuantity.value) - productInCartQuantity;
    
    if (Number(inputQuantity.value) > productQuantity) {
        document.getElementById(IdButton).setAttribute("disabled", "disabled");
        cartNotify.innerHTML = html;
    }
    else {
        document.getElementById(IdButton).removeAttribute("disabled");
        cartNotify.innerHTML = '';
    }
}

function changeItem(id) {
    document.getElementById(`star${id}`).classList.remove("bi-star");
    document.getElementById(`star${id}`).classList.add("bi-star-fill");
}
function rechangeItem(id) {
    document.getElementById(`star${id}`).classList.remove("bi-star-fill");
    document.getElementById(`star${id}`).classList.add("bi-star");
    
}
