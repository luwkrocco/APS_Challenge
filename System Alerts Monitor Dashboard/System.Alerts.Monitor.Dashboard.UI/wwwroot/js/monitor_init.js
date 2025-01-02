
var systems = {};

function Init(refresh = false) {

    //INIT DATA
    if (refresh) systems = {};

    // Process data for system AlertType widgets
    monitoringData = SortData(monitoringData);
    monitoringData
        //.sort((a, b) => a.SystemName.localeCompare(b.SystemName))
        .forEach(function (data) {
            if (!systems[data.SystemName]) {
                systems[data.SystemName] = {
                    latestAlertType: data.AlertType,
                    latestDatetime: data.Datetime,
                    latestMessage: data.FurtherDetails//`${data.SystemName}: ${data.FurtherDetails} - ${data.AlertType}`
                };
            } else if (new Date(data.Datetime) > new Date(systems[data.SystemName].latestDatetime)) {
                systems[data.SystemName] = {
                    latestAlertType: data.AlertType,
                    latestDatetime: data.Datetime,
                    latestMessage: data.FurtherDetails//`${data.SystemName}: ${data.FurtherDetails} - ${data.AlertType}`
                };
            }
        });

    // INIT SECTIONS
    InitFilter();
    InitTop();
    InitMid();
    InitLow();
}

function InitFilter() {
    // Populate system filter dropdowns
    var systemNameFilter = document.getElementById('systemNameFilter');
    systemNameFilter.innerHTML = '';

    var systemFilter = document.getElementById('systemFilter');
    systemFilter.innerHTML = '';

    // ALL OPTION
    var optionAll = document.createElement('option');
    optionAll.value = 'ALL';
    optionAll.textContent = 'ALL';
    systemNameFilter.appendChild(optionAll);;
    systemFilter.appendChild(optionAll.cloneNode(true));

    //SYSTEM ENTRIES
    Object.keys(systems).forEach(function (system) {
        var option1 = document.createElement('option');
        option1.value = system;
        option1.textContent = system;
        systemNameFilter.appendChild(option1);

        var option2 = option1.cloneNode(true);
        systemFilter.appendChild(option2);
    });
}

//////////////////////////////////////////////////////////////////////////////////////////////
// TOP PART LOGIC

function InitTop() {
    // Render system AlertType widgets
    var systemAlertTypeWidgets = document.getElementById('systemAlertTypeWidgets');
    systemAlertTypeWidgets.innerHTML = '';

    Object.keys(systems).forEach(function (system) {
        var widget = document.createElement('div');
        widget.className = 'card m-2';
        widget.style.width = '200px';
        widget.innerHTML = `
                                        <div class="card-body">
                                            <h5 class="card-title">${system}</h5>
                                            <p class="card-text">
                                                <span>${GetFormattedDate(systems[system].latestDatetime)}</span>
                                                <br/>
                                                <span class="badge bg-${GetAlertTypeColor(systems[system].latestAlertType)}">
                                                    ${systems[system].latestAlertType}
                                                </span>
                                            </p>
                                        </div>
                                    `;
        widget.addEventListener('click', function () { ShowModalMessage(systems[system].latestDatetime, system, systems[system].latestAlertType, systems[system].latestMessage); });
        systemAlertTypeWidgets.appendChild(widget);
    });
}

//////////////////////////////////////////////////////////////////////////////////////////////
// MIDDLE PART LOGIC

function InitMid() {
    // Process and render top 5 messages per system
    var top5MessagesAccordion = document.getElementById('top5MessagesAccordion');
    top5MessagesAccordion.innerHTML = '';

    var messagesBySystem = {};

    monitoringData.forEach(function (data) {
        if (!messagesBySystem[data.SystemName]) {
            messagesBySystem[data.SystemName] = [];
        }
        messagesBySystem[data.SystemName].push(data);
    });

    Object.keys(messagesBySystem).forEach(function (system, index) {
        var systemMessages = messagesBySystem[system];
        //systemMessages.sort((a, b) => new Date(b.Datetime) - new Date(a.Datetime));
        systemMessages = SortData(systemMessages);

        var accordionItem = document.createElement('div');
        accordionItem.className = 'accordion-item';
        accordionItem.innerHTML = `
                                <h2 class="accordion-header d-flex" id="heading${index}">
                                    <button class="accordion-button ${index === 0 ? '' : 'collapsed'} flex-grow-1" type="button" data-bs-toggle="collapse" data-bs-target="#collapse${index}" aria-expanded="${index === 0 ? 'true' : 'false'}" aria-controls="collapse${index}">
                                        ${system}
                                    </button>
                                    <button class="btn btn-sm btn-outline-secondary export-csv me-2" data-system="${system}" type="button">
                                        Export CSV
                                    </button>
                                </h2>
                                <div id="collapse${index}" class="accordion-collapse collapse ${index === 0 ? 'show' : ''}" aria-labelledby="heading${index}" data-bs-parent="#top5MessagesAccordion">
                                    <div class="accordion-body">
                                        <table class="table table-hover">
                                            <thead>
                                                <tr class="">
                                                    <th class="col-2">Reported At</th>
                                                    <th class="col-2">Status</th>
                                                    <th class="col-8">Details</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            `;

        var tbody = accordionItem.querySelector('tbody');

        systemMessages.slice(0, 5).forEach(function (message, index) {
            var row = document.createElement('tr');
            row.innerHTML = `
                                    <td>${GetFormattedDate(message.Datetime)}</td>
                                    <td><span class="badge bg-${GetAlertTypeColor(message.AlertType)}">${message.AlertType}</span></td>
                                    <td>${message.FurtherDetails}</td>
                                `;
            row.style.cursor = 'pointer';
            row.addEventListener('click', function () { ShowModalMessage(message.Datetime, message.SystemName, message.AlertType, message.FurtherDetails); });
            tbody.appendChild(row);
        });

        top5MessagesAccordion.appendChild(accordionItem);
    });

    // Add event listeners for export buttons
    document.querySelectorAll('.export-csv').forEach(button => {
        button.addEventListener('click', ExportData);
    });
}

//////////////////////////////////////////////////////////////////////////////////////////////
// BOTTOM PART LOGIC

function InitLow() {
    // Render message log
    var messageLog = document.getElementById('messageLog');

    function renderMessageLog(filter = 'ALL') {
        messageLog.innerHTML = ''; // Clear existing log
        logData.forEach(function (data) {
            if (filter === 'ALL' || data.SystemName === filter) {
                var logEntry = document.createElement('div');
                logEntry.textContent = `${GetFormattedDate(data.Datetime)} - ${data.SystemName} - ${data.AlertType}: ${data.FurtherDetails}`;
                messageLog.appendChild(logEntry);
            }
        });
    }

    // Initial render of message log
    renderMessageLog();

    // Add event listener for system filter
    systemFilter.addEventListener('change', function () {
        renderMessageLog(this.value);
    });
}
