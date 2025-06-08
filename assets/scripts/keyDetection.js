// Key detection
let hasKey = false;

async function fetchPlayerInventory() {
    let response;
    try {
        response = await fetch("/api/PlayerInventory/GetUserInventoryItems", {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            }
        });

        let jsonInventory = await response.json();

        jsonInventory.forEach(item => {
            if (item.name == "key" || item.name == "key2") {
                console.log(item.name);
                hasKey = true;
            }
        });

    } catch (error) {
        console.error("Error getting inventory:", error);
    }
}

function keyClicked() {
    console.log("keyclicked");
    hasKey = true;
}

document.getElementById("keyTemplate").addEventListener("click", () => {
    fetchPlayerInventory();
});


document.getElementById("nextRoomButton").addEventListener("click", () => {
    if (hasKey) {
        window.location.href = "room2"
    }
    else {
        alert("Need some kind of key to access this room");
    }
});
