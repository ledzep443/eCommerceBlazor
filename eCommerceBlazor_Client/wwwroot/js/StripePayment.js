redirectToCheckout = function (sessionId) {
    var stripe = Stripe("");
    stripe.redirectToCheckout({
        sessionId: sessionId
    });
}