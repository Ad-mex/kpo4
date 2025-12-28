let user_id = "";

const userIdInput = document.getElementById("userIdInput");
const changeIdBtn = document.getElementById("changeIdBtn");

const balanceValue = document.getElementById("balanceValue");
const createAccountBtn = document.getElementById("createAccountBtn");

async function fetchUserId() {
    try {
        const response = await fetch("http://localhost:5150/user_id");
        user_id = await response.text();
        userIdInput.value = user_id;

        fetchBalance();
        fetchOrders();
    } catch (e) {
        console.error(e);
        userIdInput.value = "Ошибка";
    }
}

userIdInput.addEventListener("input", () => {
    user_id = userIdInput.value;
    fetchBalance();
});

changeIdBtn.addEventListener("click", fetchUserId);

createAccountBtn.addEventListener("click", async () => {
    try {
        createAccountBtn.disabled = true;

        await fetch(
            `http://localhost:5150/create_account?user_id=${encodeURIComponent(user_id)}`,
            { method: "POST" }
        );

        fetchBalance();
    } catch (e) {
        console.error(e);
        balanceValue.textContent = "Ошибка создания счёта";
    } finally {
        createAccountBtn.disabled = false;
    }
});

const depositBtn = document.getElementById("depositBtn");
const depositModal = document.getElementById("depositModal");
const depositAmountInput = document.getElementById("depositAmount");
const depositConfirmBtn = document.getElementById("depositConfirmBtn");
const depositCancelBtn = document.getElementById("depositCancelBtn");

async function fetchBalance() {
    if (!user_id) return;

    balanceValue.textContent = "Загрузка...";
    createAccountBtn.style.display = "none";
    depositBtn.style.display = "none";

    try {
        const response = await fetch(
            `http://localhost:5150/balance?user_id=${encodeURIComponent(user_id)}`
        );

        if (response.status === 404) {
            balanceValue.textContent = "Счёт не создан";
            createAccountBtn.style.display = "block";
            return;
        }

        if (!response.ok) {
            throw new Error("Ошибка получения баланса");
        }

        const balance = await response.text();
        balanceValue.textContent = balance;

        depositBtn.style.display = "block";
    } catch (e) {
        console.error(e);
        balanceValue.textContent = "Ошибка";
    }
}

depositBtn.addEventListener("click", () => {
    depositAmountInput.value = "";
    depositModal.style.display = "flex";
});

depositCancelBtn.addEventListener("click", () => {
    depositModal.style.display = "none";
});

depositConfirmBtn.addEventListener("click", async () => {
    const amount = parseFloat(depositAmountInput.value);
    if (isNaN(amount) || amount <= 0) {
        alert("Введите корректную сумму");
        return;
    }

    try {
        await fetch(
            `http://localhost:5150/add_money?user_id=${encodeURIComponent(user_id)}&amount=${encodeURIComponent(amount)}`,
            { method: "POST" }
        );

        depositModal.style.display = "none";
        fetchBalance();
    } catch (e) {
        console.error(e);
        alert("Ошибка пополнения");
    }
});


fetchUserId();


const ordersContainer = document.getElementById("ordersContainer");
const refreshOrdersBtn = document.getElementById("refreshOrdersBtn");

const orderModal = document.getElementById("orderModal");
const orderModalContent = document.getElementById("orderModalContent");
const orderDescription = document.getElementById("orderDescription");

async function fetchOrders() {
    if (!user_id) return;
    ordersContainer.textContent = "Загрузка...";

    try {
        const response = await fetch(`http://localhost:5150/orders?user_id=${encodeURIComponent(user_id)}`);
        if (!response.ok) throw new Error("Ошибка получения заказов");

        const orders = await response.json();

        renderOrders(orders);
    } catch (e) {
        console.error(e);
        ordersContainer.textContent = "Ошибка загрузки заказов";
    }
}

function renderOrders(orders) {
    ordersContainer.innerHTML = "";

    if (!orders.length) {
        ordersContainer.textContent = "Заказов нет";
        return;
    }

    orders.forEach(order => {
        const orderDiv = document.createElement("div");
        orderDiv.className = "orderItem";

        const orderIdDiv = document.createElement("div");
        orderIdDiv.className = "orderId";
        orderIdDiv.textContent = order.id;

        const orderInfoDiv = document.createElement("div");
        orderInfoDiv.className = "orderInfo";
        orderInfoDiv.textContent = `Сумма: ${order.amount}`;

        orderDiv.appendChild(orderIdDiv);
        orderDiv.appendChild(orderInfoDiv);

        orderDiv.addEventListener("click", async (e) => {
            e.stopPropagation();
            orderDescription.textContent = "Загрузка...";
            orderStatus.textContent = "";

            orderModal.style.display = "flex";

            orderDescription.textContent = order.description || "(Описание отсутствует)";

            try {
                const statusResp = await fetch(
                    `http://localhost:5150/status?order_id=${encodeURIComponent(order.id)}`
                );

                if (!statusResp.ok) throw new Error("Ошибка получения статуса");

                const status = await statusResp.text();

                orderStatus.textContent = `Status: ${status}`;

                orderStatus.className = "";
                if (status === "NEW") orderStatus.classList.add("status-new");
                else if (status === "CANCELLED") orderStatus.classList.add("status-cancelled");
                else if (status === "FINISHED") orderStatus.classList.add("status-finished");
            } catch (err) {
                console.error(err);
                orderStatus.textContent = "Ошибка получения статуса";
            }
        });


        ordersContainer.appendChild(orderDiv);
    });
}

refreshOrdersBtn.addEventListener("click", fetchOrders);

orderModal.addEventListener("click", () => {
    orderModal.style.display = "none";
});

orderModalContent.addEventListener("click", (e) => {
    e.stopPropagation();
});

fetchOrders();

const createOrderBtn = document.getElementById("createOrderBtn");
const orderCreateModal = document.getElementById("orderCreateModal");
const orderDescriptionInput = document.getElementById("orderDescriptionInput");
const orderAmountInput = document.getElementById("orderAmountInput");
const orderCreateConfirmBtn = document.getElementById("orderCreateConfirmBtn");
const orderCreateCancelBtn = document.getElementById("orderCreateCancelBtn");

function updateCreateOrderButton() {
    if (depositBtn.style.display === "block") {
        createOrderBtn.style.display = "block";
    } else {
        createOrderBtn.style.display = "none";
    }
}

createOrderBtn.addEventListener("click", () => {
    orderDescriptionInput.value = "";
    orderAmountInput.value = "";
    orderCreateModal.style.display = "flex";
});

orderCreateCancelBtn.addEventListener("click", () => {
    orderCreateModal.style.display = "none";
});

orderCreateConfirmBtn.addEventListener("click", async () => {
    const amount = parseFloat(orderAmountInput.value);
    const description = orderDescriptionInput.value || "";

    if (isNaN(amount) || amount <= 0) {
        alert("Введите корректную сумму");
        return;
    }

    try {
        await fetch(
            `http://localhost:5150/create_order?user_id=${encodeURIComponent(user_id)}&amount=${encodeURIComponent(amount)}&description=${encodeURIComponent(description)}`,
            { method: "POST" }
        );

        orderCreateModal.style.display = "none";

        fetchBalance();
        fetchOrders();

        showToast("Заказ отправлен на обработку");
    } catch (e) {
        console.error(e);
        alert("Ошибка создания заказа");
    }

});

orderCreateModal.addEventListener("click", () => {
    orderCreateModal.style.display = "none";
});

orderCreateModal.querySelector(".modal-content").addEventListener("click", (e) => {
    e.stopPropagation();
});

async function fetchBalance() {
    if (!user_id) return;

    balanceValue.textContent = "Загрузка...";
    createAccountBtn.style.display = "none";
    depositBtn.style.display = "none";
    createOrderBtn.style.display = "none";

    try {
        const response = await fetch(
            `http://localhost:5150/balance?user_id=${encodeURIComponent(user_id)}`
        );

        if (response.status === 404) {
            balanceValue.textContent = "Счёт не создан";
            createAccountBtn.style.display = "block";
            return;
        }

        if (!response.ok) throw new Error("Ошибка получения баланса");

        const balance = await response.text();
        balanceValue.textContent = balance;

        depositBtn.style.display = "block";
        updateCreateOrderButton();
    } catch (e) {
        console.error(e);
        balanceValue.textContent = "Ошибка";
    }
}

const refreshBalanceBtn = document.getElementById("refreshBalanceBtn");

userIdInput.addEventListener("input", () => {
    user_id = userIdInput.value;

    ordersContainer.innerHTML = "";
    ordersContainer.textContent = "Загрузка...";

    fetchBalance();
    fetchOrders();
});

refreshBalanceBtn.addEventListener("click", fetchBalance);

async function fetchBalance() {
    if (!user_id) return;

    balanceValue.textContent = "Загрузка...";
    createAccountBtn.style.display = "none";
    depositBtn.style.display = "none";
    createOrderBtn.style.display = "none";

    try {
        const response = await fetch(
            `http://localhost:5150/balance?user_id=${encodeURIComponent(user_id)}`
        );

        if (response.status === 404) {
            balanceValue.textContent = "Счёт не создан";
            createAccountBtn.style.display = "block";
            return;
        }

        if (!response.ok) throw new Error("Ошибка получения баланса");

        const balance = await response.text();
        balanceValue.textContent = balance;

        depositBtn.style.display = "block";
        updateCreateOrderButton();
    } catch (e) {
        console.error(e);
        balanceValue.textContent = "Ошибка";
    }
}

function showToast(message) {
    const toast = document.getElementById("toast");
    toast.textContent = message;
    toast.style.display = "block";
    toast.style.opacity = "1";

    setTimeout(() => {
        toast.style.opacity = "0";
        setTimeout(() => {
            toast.style.display = "none";
        }, 300);
    }, 1000);
}
