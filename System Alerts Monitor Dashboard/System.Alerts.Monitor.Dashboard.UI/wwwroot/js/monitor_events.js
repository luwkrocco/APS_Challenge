document.getElementById('sortByDatetime').addEventListener('click', function () {
    CurrentSortCriteria = 'Datetime';
    UpdateSortButtons(CurrentSortCriteria);
    Init(false);
});

document.getElementById('sortByStatus').addEventListener('click', function () {
    CurrentSortCriteria = 'AlertType';
    UpdateSortButtons(CurrentSortCriteria);
    Init(false);
});


// Add event listener for filter form
document.getElementById('filterForm').addEventListener('submit', async function (e) {
    e.preventDefault();
    CallFetchFilterAndRefresh()
});


document.getElementById('autoRefreshToggle').addEventListener('click', toggleAutoRefresh);