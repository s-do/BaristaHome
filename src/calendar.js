import { Calendar } from '@fullcalendar/core';
import interactionPlugin from '@fullcalendar/interaction';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import listPlugin from '@fullcalendar/list';
import './calendar.css';

document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('sharedCalendar');

    var calendar = new Calendar(calendarEl, {
        plugins: [interactionPlugin, dayGridPlugin, timeGridPlugin, listPlugin],
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
        },

        height: 1000,
        allDaySlot: false,
        eventStartEditable: false,
        navLinks: true, // can click day/week names to navigate views
        dayMaxEvents: true, // allow "more" link when too many events
        eventMaxStack: 3,
        eventDisplay: 'list-item',
        editable: true,

        // owners & managers are only allowed to create shifts
        selectable: true,
        select: addShift, 
        // parsing the shifts from the store and then updating it on the view
        /*events: '/Calendar/GetShifts',*/
        eventClick: updateShift,
        events: [
            {
                id: 1,
                title: 'Shift1',
                start: '2022-10-12T06:30:00',
                end: '2022-10-12T17:30:00'
            },
            {
                id: 2,
                title: 'Shift2',
                start: '2022-10-12T07:30:00',
                end: '2022-10-12T16:30:00'
            },
            {
                id: 3,
                title: 'Shift3',
                start: '2022-10-12T08:00:00',
                end: '2022-10-12T18:30:00'
            },
            {
                id: 4,
                title: 'Click for Google',
                url: 'http://google.com/',
                start: '2022-10-28'
            }
        ]

        
    });
    calendar.render();
});

function addShift() {
    $('#shiftModal').modal('show');
}

function updateShift(info) {
    /* TODO: Figure out how to get event property values and put them into the html input tags (as well as formatting the time correctly)*/
    // this doesn't work
    $('#startTime').val(info.event.start);
    $('#endTime').val(info.event.end);
    $('#shiftId').val(info.event.id);
    // hardcoding does
    $('#workerId').val(2);

    /*var getTxtValue = info.event.id; 
    $("#myInputHidden").val(info.event.id);*/

    $('#editModal').modal('show');
    //alert('TODO: UPDATE A SHIFT');
}


