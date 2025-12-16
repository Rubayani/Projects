import fetch from "node-fetch";

async function getPrices(url_name) {
    const url = `https://api.warframe.market/v1/items/${url_name}/orders`;

    const res = await fetch(url);
    const data = await res.json();

    const orders = data.payload.orders;

    const sellOrders = orders.filter(o =>
        o.order_type === "sell" &&
        o.platform === "pc" &&
        o.visible
    );

    const prices = sellOrders.map(o => o.platinum).sort((a, b) => a - b);

    return prices;
}

const mod = "blind_rage";

getPrices(mod).then(prices => {
    console.log("Prices:", prices);
    console.log("Lowest:", prices[0]);
});
