import { Calendar } from '@fullcalendar/core';
import interactionPlugin from '@fullcalendar/interaction';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import listPlugin from '@fullcalendar/list';
import './calendar.css';
//const formatDate = date => date === null ? '' : moment(date).format("MM/DD/YYYY h:mm A");

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
        eventClick: updateShift,
        events: '/Calendar/GetShifts',

/*        events: [
            {
                id: 1,
                title: 'Shift1',
                start: '2022-10-12T06:00:00',
                end: '2022-10-12T17:30:00',
                UserId: 42
            },
            {
                id: 2,
                title: 'Shift2',
                start: '2022-10-12T07:30:00',
                end: '2022-10-12T16:30:00',
                UserId: 69
            },
            {
                id: 3,
                title: 'Shift3',
                start: '2022-10-12T08:00:00',
                end: '2022-10-12T18:30:00'
            },
            {
                id: 4,
                title: 'A Google Link',
                url: 'http://google.com/',
                start: '2022-10-28'
            }
        ]*/
        
    });
    calendar.render();
});

function addShift(info) {
    var d = new Date(info.start)
    // the godly one liner to convert javascript's Date object into a DateFormat (YYYY-MM-DD HH:MM)
    d = d.getFullYear() + "-" + ('0' + (d.getMonth() + 1)).slice(-2) + "-" + ('0' + d.getDate()).slice(-2) + " " + ('0' + d.getHours()).slice(-2) + ":" + ('0' + d.getMinutes()).slice(-2) + ":" + ('0' + d.getSeconds()).slice(-2);
    $('#start').val(d);
    $('#end').val(d);
    $('#shiftModal').modal('show');
}

function updateShift(info) {
    // using Fullcalendar's Date object methods to format start/end time into dateformat for input box (example: 2013-03-18 03:00)
    var startMonth = info.event.start.getMonth() + 1; // getMonth() is 0 based so must add 1
    var endMonth = info.event.end.getMonth() + 1;
    // adding leading 0's for single digit values to follow formatting rules
    if (startMonth < 10) {
        startMonth = '0' + startMonth; 
    }
    if (endMonth < 10) {
        endMonth = '0' + endMonth;
    }

    // Date
    var startDate = info.event.start.getDate();
    var endDate = info.event.end.getDate();
    if (startDate < 10) {
        startDate = '0' + startDate;
    }
    if (endDate < 10) {
        endDate = '0' + endDate;
    }

    // Hour
    var startHour = info.event.start.getHours();
    var endHour = info.event.end.getHours();
    if (startHour < 10) {
        startHour = '0' + startHour;
    }
    if (endHour < 10) {
        endHour = '0' + endHour;
    }

    // Minute
    var startMinute = info.event.start.getMinutes();
    var endMinute = info.event.end.getMinutes()
    if (startMinute < 10) {
        startMinute = '0' + startMinute;
    }
    if (endMinute < 10) {
        endMinute = '0' + endMinute;
    }

    $('#editHeader').text("Edit Shift - " + info.event.title);

    // inserts value into dateformat (YYYY-MM-DD HH:MM)
    $('#startTime').val(info.event.start.getFullYear() + '-' + startMonth + '-' + startDate + ' ' + startHour + ':' + startMinute);

    $('#endTime').val(info.event.end.getFullYear() + '-' + endMonth + '-' + endDate + ' ' + endHour + ':' + endMinute)

    // setting ids to query and find shift to edit after user changes time
    $('#shiftId').val(info.event.id); // works when u set the viewmodel to the exact naming of the event property
    $('#workerId').val(info.event.extendedProps.userId); // figure out how to call custom properties 

   

    $('#editModal').modal('show');
    //alert('TODO: UPDATE A SHIFT');
}