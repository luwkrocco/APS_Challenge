
let idleTimer;
let idleState = false;
let idleTime = 3000; // X seconds ( 1000 == 1 second )

function AutoRefreshEnabled() { return document.getElementById('autoRefreshToggle').checked; }
function toggleAutoRefresh() {
    clearTimeout(idleTimer);

    if (AutoRefreshEnabled())
        idleTimer = setTimeout(goIdle, idleTime);
}

function resetIdleTimer() {
    toggleAutoRefresh();

    if (idleState) {
        // If the user was previously idle, they are now active
        console.log("User is now ACTIVE");
        idleState = false;
    }
}

function goIdle() {
    idleState = true;
    console.log("User is now IDLE");

    // Dislpay Refreshing Toast
    ShowNotification('Refreshing Data.');

    // Call your function here after X seconds of inactivity
    CallFetchFilterAndRefresh();

    // recheck if need to refresh after X seconds of inactivity
    toggleAutoRefresh();
}

// Set up the event listeners
window.onload = resetIdleTimer; // On initial load
window.onmousemove = resetIdleTimer;
window.onmousedown = resetIdleTimer; // Catches touchscreen presses as well
window.ontouchstart = resetIdleTimer;
window.onclick = resetIdleTimer; // For any clicks
window.onkeydown = resetIdleTimer; // For keyboard activity
window.onscroll = resetIdleTimer; // For scrolling

// Optionally, handle visibility changes (e.g., tab switching)
document.addEventListener("visibilitychange", function () {
    if (document.hidden) {
        clearTimeout(idleTimer); // Clear the timer if the tab is hidden
    } else {
        resetIdleTimer(); // Reset the timer when the tab becomes visible again
    }
});