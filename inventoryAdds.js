document.addEventListener("DOMContentLoaded", function () {
    const clickables = document.querySelectorAll(".clickable");

    clickables.forEach(element => {
        element.addEventListener("click", async function () {
            const itemId = this.id;
            await AddItemToInventory(itemId);
        });
    });
});

window.AddItemToInventory = async function AddItemToInventory(itemId) {
    // Skip adding door lol
    if (itemId === "nextRoomButton") return;

    try {
        const response = await fetch("/api/PlayerInventory/AddToUserInventory", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(itemId)
        });

        if (!response.ok) {
            console.error("Failed to add item to inventory");
        } else {
            console.log(`Item "${itemId}" added to inventory.`);
        }
    } catch (error) {
        console.error("Error sending item to server:", error);
    }
}