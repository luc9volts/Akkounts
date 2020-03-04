const pubKey = `-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoBttaXwRoI1Fbcond5mS
7QOb7X2lykY5hvvDeLJelvFhpeLnS4YDwkrnziM3W00UNH1yiSDU+3JhfHu5G387
O6uN9rIHXvL+TRzkVfa5iIjG+ap2N0/toPzy5ekpgxBicjtyPHEgoU6dRzdszEF4
ItimGk5ACx/lMOvctncS5j3uWBaTPwyn0hshmtDwClf6dEZgQvm/dNaIkxHKV+9j
Mn3ZfK/liT8A3xwaVvRzzuxf09xJTXrAd9v5VQbeWGxwFcW05oJulSFjmJA9Hcmb
DYHJT+sG2mlZDEruCGAzCVubJwGY1aRlcs9AQc1jIm/l8JwH7le2kpk3QoX+gz0w
WwIDAQAB
-----END PUBLIC KEY-----`;

new Vue({
    el: '#app',
    data: {
        options: [
            { text: 'CREDIT CARD', value: 'CREDIT_CARD' },
            { text: 'BOLETO', value: 'BOLETO' }
        ],
        ExpirationMonth: "",
        ExpirationYear: "",
        Brand: "",
        LocationLastPayment: "",
        Amount: 0,
        Type: "CREDIT_CARD",
        Client: { Id: 5 },
        Buyer: {
            Cpf: "",
            Name: "",
            Email: ""
        },
        Card: {
            Number: "",
            HolderName: "",
            ExpirationDate: "",
            Cvv: ""
        }
    },
    methods: {
        creditCardValid: function () {
            var cc = new Moip.CreditCard({
                number: this.Card.Number,
                cvc: this.Card.Cvv,
                expMonth: this.ExpirationMonth,
                expYear: this.ExpirationYear,
                pubKey: pubKey
            });
            this.Brand = cc.cardType(cc.number);
            return cc.isValid();
        },
        onAdd: function (event) {
            var requestObj = {
                Amount: this.Amount,
                Type: this.Type,
                Client: this.Client,
                Buyer: this.Buyer
            };

            if (this.Type === "CREDIT_CARD") {
                if (!this.creditCardValid()) {
                    alert('Invalid credit card. Verify parameters: number, cvc, expiration Month, expiration Year');
                    return;
                }
                this.Card.ExpirationDate = new Date(this.ExpirationYear, this.ExpirationMonth - 1);
                requestObj.Card = this.Card;
            }

            const vmodel = this;

            axios.post('../api/v1/payments', requestObj)
                .then(function (response) {
                    vmodel.LocationLastPayment = response.headers.location;
                    alert('Result of process: ' + response.data.toString());
                })
                .catch(function (error) {
                    alert(formatErrorMsg(error));
                });
        },
        onGet: function (event) {
            axios.get(this.LocationLastPayment)
                .then(function (response) {
                    alert(JSON.stringify(response.data));
                })
                .catch(function (error) {
                    alert(formatErrorMsg(error));
                });
        }
    }
});

function formatErrorMsg(errorResponse) {
    var errMsg = "";
    for (e in errorResponse.response.data) {
        errMsg += errorResponse.response.data[e] + '\n';
    }
    return errMsg;
}
