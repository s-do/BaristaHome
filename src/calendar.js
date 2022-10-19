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
        selectable: true,
        select: addShift, 
        eventClick: updateShift,
        events: '/Calendar/GetShifts', // json feed
        // event parsing
        /*events: [
            {
                id: 1,
                title: 'Shift1',
                start: '2022-10-12T06:00:00',
                end: '2022-10-12T17:30:00',
                UserId: 42
            },
            {
                id: 2,
                title: 'A Google Link',
                url: 'http://google.com/',
                start: '2022-10-28'
            }
        ]*/
    });
    calendar.render();
});

$('#deleteShift').click(() => {
    $("#editModal").modal("hide");
    $('#deleteModal').modal('show');
})

$('#noButton').click(() => {
    $("#editModal").modal("show");
    $('#deleteModal').modal('hide');
})

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
    var s = new Date(info.event.start);
    var e = new Date(info.event.end);

    // months is 0 indexed, putting 0's in front of single digit numbers
    s = s.getFullYear() + "-" + ('0' + (s.getMonth() + 1)).slice(-2) + "-" + ('0' + s.getDate()).slice(-2) + " " + ('0' + s.getHours()).slice(-2) + ":" + ('0' + s.getMinutes()).slice(-2) + ":" + ('0' + s.getSeconds()).slice(-2);
    e = e.getFullYear() + "-" + ('0' + (e.getMonth() + 1)).slice(-2) + "-" + ('0' + e.getDate()).slice(-2) + " " + ('0' + e.getHours()).slice(-2) + ":" + ('0' + e.getMinutes()).slice(-2) + ":" + ('0' + e.getSeconds()).slice(-2);

    // jQuery to manipulate HTML fields
    $('#editHeader').text("Edit Shift - " + info.event.title);
    $('#deleteHeader').text("Deleting Shift for " + info.event.title);

    // inserts value into dateformat (YYYY-MM-DD HH:MM)
    $('#startTime').val(s);
    $('#endTime').val(e);

    // setting ids to query and find shift to edit after user changes time
    $('#shiftId').val(info.event.id); 
    $('#deleteShiftId').val(info.event.id);

    // how to call custom properties 
    $('#workerId').val(info.event.extendedProps.userId); 

    $('#editModal').modal('show');
}