// 當用戶點擊按鈕時觸發
document.getElementById('sendPostRequest').addEventListener('click', function (event) {
    // 構造需要發送的資料
    var data = {
        orderId: '12345',
    };

    fetch(env.baseHost + "/api/order/create", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data) // 將資料轉換為 x-www-form-urlencoded 格式
    })
        .then(response => response.text())  // 期望回應的內容是文本（HTML）
        .then(html => {
            document.getElementById('checkoutContent').innerHTML = html;
            document.getElementById("checkoutForm").submit();
        })
        .catch(error => {
            document.getElementById('checkoutContent').innerHTML = '<p>An error occurred while processing your request.</p>';
        });
});

