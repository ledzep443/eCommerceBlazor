redirectToCheckout = function (sessionId) {
    var stripe = Stripe("pk_test_51Kh47pL6lk7SigOZVbajneG7rUTyfQycyUx4iWZ3AJum7iqsIbY20khdyvn4UxUpwfmnwVSdD8BXsP3wQxS5CJdI00vydotMfg");
    stripe.redirectToCheckout({
        sessionId: sessionId
    });
}