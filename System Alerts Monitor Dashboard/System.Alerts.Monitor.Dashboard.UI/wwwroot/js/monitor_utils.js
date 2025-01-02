
//////////////////////////////////////////////////////////////////////////////////////////////
// MISC METHODS

function GetFormattedDate(inputDateTime) {
    var datetime = new Date(inputDateTime);
    var day = String(datetime.getDate()).padStart(2, "0");
    var month = String(datetime.getMonth() + 1).padStart(2, "0"); // Months are 0-based
    var year = datetime.getFullYear();
    var hours = String(datetime.getHours()).padStart(2, "0");
    var minutes = String(datetime.getMinutes()).padStart(2, "0");
    var seconds = String(datetime.getSeconds()).padStart(2, "0");

    const formattedDate = `${day}-${month}-${year} ${hours}:${minutes}:${seconds}`;
    return formattedDate;
}

function GetAlertTypeColor(AlertType) {
    switch (AlertType) {
        case 'OK': return 'success';
        case 'WARNING': return 'warning';
        case 'ERROR': return 'danger';
        default: return 'secondary';
    }
}


//////////////////////////////////////////////////////////////////////////////////////////////
// SORTING LOGIC

var CurrentSortCriteria = 'Datetime'; // Datetime or AlertType

function SortData(data) {
    data = data ?? [];

    const statusOrder = { 'ERROR': -1, 'WARNING': 0, 'OK': 1 };
    switch (CurrentSortCriteria) {
        case 'Datetime':
            data.sort((a, b) => //new Date(b.Datetime) - new Date(a.Datetime));
            {
                // 1. Sort by SystemName
                const nameComparison = a.SystemName.localeCompare(b.SystemName);
                if (nameComparison !== 0) { return nameComparison; }

                // 2. If SystemNames are the same, sort by Datetime (descending)
                return new Date(b.Datetime) - new Date(a.Datetime);
            });
            break;
        case 'AlertType':
            data.sort((a, b) => //statusOrder[a.AlertType.toLowerCase()] - statusOrder[b.AlertType.toLowerCase()]);
            {
                // 1. Sort by SystemName
                const nameComparison = a.SystemName.localeCompare(b.SystemName);
                if (nameComparison !== 0) { return nameComparison; }

                // 2. If SystemNames are the same, sort by Datetime (descending)
                return statusOrder[a.AlertType.toUpperCase()] - statusOrder[b.AlertType.toUpperCase()];
            });
            break;
    }
    return data;
}

function UpdateSortButtons(activeCriteria) {
    document.getElementById('sortByDatetime').classList.toggle('active', activeCriteria === 'Datetime');
    document.getElementById('sortByStatus').classList.toggle('active', activeCriteria === 'AlertType');
}

//////////////////////////////////////////////////////////////////////////////////////////////
// FILTER LOGIC

async function CallFetchFilterAndRefresh() {
    var fromDate = document.getElementById('fromDate').value;
    var toDate = document.getElementById('toDate').value;
    var systemName = document.getElementById('systemNameFilter').value;

    var filter = {
        FromDate: fromDate,
        ToDate: toDate,
        SystemName: systemName
    };

    try {
        var filteredData = await FetchFilteredData(filter);
        var fdata = filteredData.AlertData ?? [];
        logData = monitoringData = fdata;

        if (fdata.length > 0) Init(true);
        else ShowNotification("No Data retrieved from Server. <br/>Please check with the System Admin.");
    }
    catch (error) {
        console.error('Error fetching filtered data:', error);
        ShowNotification(error, true);
    }
}

// Function to fetch filtered data from the server
async function FetchFilteredData(filter) {
    try {
        const response = await fetch('/Dashboard/FilterData', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(filter),
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        return await response.json();
    } catch (error) {
        console.error('Error fetching filtered data:', error);
        throw error;
    }
}


//////////////////////////////////////////////////////////////////////////////////////////////
// EXPORT DATA LOGIC

async function ExportData(e) {
    e.stopPropagation(); // Prevent the accordion from toggling when clicking the export button
    const system = this.getAttribute('data-system');

    console.log(system);

    var fromDate = document.getElementById('fromDate').value;
    var toDate = document.getElementById('toDate').value;
    var systemName = system;

    var filter = {
        FromDate: fromDate,
        ToDate: toDate,
        SystemName: systemName
    };
    
    try {
        var response = await FetchFilteredData(filter);
        var exportData = response.AlertData;
        var compiledData = CompileCSV(exportData);

        const blob = new Blob([compiledData], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        if (link.download !== undefined) {
            const url = URL.createObjectURL(blob);
            link.setAttribute('href', url);
            link.setAttribute('download', `${system}_data.csv`);
            link.style.visibility = 'hidden';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            ShowNotification(`CSV for ${system} has been downloaded`);
        } else {
            ShowNotification(`Unable to download CSV for ${system}`, true);
        }
    }
    catch (error) {
        console.error('Error fetching filtered data:', error);
    }
}

function CompileCSV(data) {
    try {
        const headers = ['Reported At', 'Status', 'System Name', 'Details'];
        const csvContent = [
            headers.join(','),
            ...data.map(item => [
                item.Datetime,
                item.AlertType,
                item.SystemName,
                item.FurtherDetails
            ].join(','))
        ].join('\n');

        return csvContent;
    }
    catch (error) { throw error; }
}

//////////////////////////////////////////////////////////////////////////////////////////////
// TOASTY - NOTIFICATION LOGIC
const notificationToast = bootstrap.Toast.getOrCreateInstance(document.getElementById('noticeToast'), { delay: 3000 });
const errorToast = bootstrap.Toast.getOrCreateInstance(document.getElementById('errorToast'), { delay: 3000 });


function ShowNotification(msg, error) {
    var tb = (error ?? false) ? 'etoast-body' : 'toast-body';
    const toastBody = document.getElementById(tb);
    toastBody.innerHTML = msg;

    // DISPLAY TOAST NOTICE
    if (error ?? false) errorToast.show();
    else notificationToast.show();
    
    // LOG TO CONSOLE
    if (error ?? false) console.log(msg);
    else console.warn(msg);    
}

function ShowModalMessage(datetime, system, status, message) {
    var modal = new bootstrap.Modal(document.getElementById('messageModal'));
    var modalBody = document.getElementById('messageModalBody');
    modalBody.innerHTML = `
                                <p><strong>System Name:</strong> ${system}</p>
                                <p><strong>Reported At:</strong> ${GetFormattedDate(datetime)}</p>
                                <p><strong>Status:</strong> <span class="badge bg-${GetAlertTypeColor(status)}">${status}</span> </p>
                                <p><strong>Details:</strong> ${message}</p>
                            `;
    modal.show();
}