import { Calendar } from '@fullcalendar/core';
//import interactionPlugin from '@fullcalendar/interaction';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import listPlugin from '@fullcalendar/list';
import './calendar.css';

document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('sharedCalendar');

    var calendar = new Calendar(calendarEl, {
        plugins: [dayGridPlugin, timeGridPlugin, listPlugin],
        headerToolbar: {
            left: 'prev,next today addShift',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
        },
        customButtons: {
            addShift: {
                text: 'add shift',
                click: addShift
            }
        },
        height: 1000,
        allDaySlot: false,
        navLinks: true, // can click day/week names to navigate views
        editable: true,
        dayMaxEvents: true, // allow "more" link when too many events
        eventMaxStack: 3,
        eventDisplay: 'list-item',
        selectable: true,
        select: updateShift, // owners & managers are only allowed this abiltiy
        // parsing the shifts from the store and then updating it on the view
        events: '/Calendar/GetShifts',
        /*events: [
            {
                title: 'Shift1',
                start: '2022-10-12T06:30:00',
                end: '2022-10-12T17:30:00'
            },
            {
                title: 'Shift2',
                start: '2022-10-12T07:30:00',
                end: '2022-10-12T16:30:00'
            },
            {
                title: 'Shift3',
                start: '2022-10-12T08:00:00',
                end: '2022-10-12T18:30:00'
            {
                title: 'Click for Google',
                url: 'http://google.com/',
                start: '2022-10-28'
            }
        ]*/
    });
    calendar.render();
});

function addShift() {
    $('#shiftModal').modal('show');
    //alert('TODO: ADD A SHIFT');
}

function updateShift() {
    alert('TODO: UPDATE A SHIFT');
}


