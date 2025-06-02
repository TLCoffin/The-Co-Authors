let inventoryItems = [];

let itemImageLinkName = {
    "book": "/assets/images/BookCAOD.png",
    "glasses": "/assets/images/GlassesCAOD.png",
    "key": "/assets/images/KeyCAOD.png",
    "necklace": "/assets/images/NecklaceCAOD.png",
    "blood": "/assets/images/BloodCAOD.png",
    "bottle": "/assets/images/BottleCAOD.png",
    "pages": "/assets/images/PagesCAOD.png",
    "key2": "/assets/images/KeyCAOD.png",
    "newspaper": "/assets/images/NewspaperCAOD.png",
    "knife": "/assets/images/KnifeCAOD.png",
    "picture": "/assets/images/PictureFrameCAOD.png"
};



async function loadItemListFromDatabase() {
    // fetch and update this list from the database
    var response;
    try {
        response = await fetch("/api/PlayerInventory/GetUserInventoryItems", {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            }
        });
    } catch (error) {
        console.error("Error getting inventory:", error);
    }

    let itemList = await response.json();

    itemList.forEach(item => {
        if (!inventoryItems.includes(item.name)) {
            console.log("Item Name: ", item.name);
            inventoryItems.push(item.name);
        }
    });
}

function addToInventory(newItem) {
    if (!inventoryItems.includes(newItem)) {
        inventoryItems.push(newItem);
        upateInventoryPageLocally();
    }
}

function upateInventoryPageLocally() {
    let cells = document.querySelectorAll(".inventoryBox td img");
    let i = 0;
    inventoryItems.forEach(item => {
        cells[i].src = itemImageLinkName[item];
        i++;
    });
}

async function updateInventoryPage() {
    let cells = document.querySelectorAll(".inventoryBox td img");
    let i = 0;
    console.log(inventoryItems);
    inventoryItems.forEach(item => {
        console.log(itemImageLinkName[item]);
        cells[i].src = itemImageLinkName[item];
        i++;
    });
}

window.onload = async function () {
    await loadItemListFromDatabase();
    await updateInventoryPage();
}