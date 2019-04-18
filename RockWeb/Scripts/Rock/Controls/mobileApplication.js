(function ($) {
    'use strict';
    window.Rock = window.Rock || {};
    Rock.controls = Rock.controls || {};

    /** JS helper for the groupScheduler */
    Rock.controls.mobileApplication = (function () {
        var exports = {
            /** initializes the javascript for the groupScheduler */
            initialize: function (options) {
                if (!options.id) {
                    throw 'id is required';
                }

                var self = this;

                var $control = $('#' + options.id);

                if ($control.length == 0) {
                    return;
                }

                self.$mobileApplication = $control;

                // initialize dragula
                var containers = [];
                debugger;
                // add the resource list as a dragular container
                containers.push($control.find('.js-mobile-blocktype-source-container')[0]);

                // add all the occurrences (locations) as dragula containers
                var targets = $control.find('.js-mobile-blocktype-target-container').toArray();

                $.each(targets, function (i) {
                    containers.push(targets[i]);
                });

                self.resourceListDrake = dragula(containers, {
                    isContainer: function (el) {
                        return false;
                    },
                    copy: function (el, source) {
                        return false;
                    },
                    accepts: function (el, target) {
                        return true;
                    },
                    invalid: function (el, handle) {
                        // ignore drag if they are clicking on the actions menu of a resource
                        var isMenu = $(el).closest('.js-resource-actions').length;
                        return isMenu;
                    },
                    ignoreInputTextSelection: true
                })
                    .on('drag', function (el) {
                        if (self.resourceScroll) {
                            // disable the scroller while dragging so that the scroller doesn't move while we are dragging
                            //self.resourceScroll.disable();
                        }
                        $('body').addClass('state-drag');
                    })
                    .on('dragend', function (el) {
                        if (self.resourceScroll) {
                            // reenable the scroller when done dragging
                            //self.resourceScroll.enable();
                        }
                        $('body').removeClass('state-drag');
                    })
                    .on('drop', function (el, target, source, sibling) {
                        if (target.classList.contains('js-mobile-blocktype-source-container')) {
                            // deal with the resource that was dragged back into the unassigned resources
                            var $unassignedResource = $(el);
                            $unassignedResource.attr('data-state', 'unassigned');

                            var blocktypeId = $unassignedResource.attr('data-blocktype-id')

                            //$.ajax({
                            //    method: "PUT",
                            //    url: scheduledPersonUnassignUrl + '?attendanceId=' + attendanceId
                            //}).done(function (scheduledAttendance) {
                            //    // after unassigning a resource, repopulate the list of unassigned resources
                            //    self.populateSchedulerResources(self.$resourceList);

                            //    // after unassigning a resource, repopulate the list of resources for the occurrence
                            //    var $occurrence = $(source).closest('.js-scheduled-occurrence');
                            //    self.populateScheduledOccurrence($occurrence);
                            //}).fail(function (a, b, c) {
                            //    // TODO
                            //})
                        }
                        else {
                            // deal with the resource that was dragged into an assigned zone
                            //var scheduledPersonAssignUrl = Rock.settings.get('baseUrl') + 'api/Attendances/ScheduledPersonAssign';
                            //var $assignedResource = $(el);
                            //$assignedResource.attr('data-state', 'assigned');
                            //var personId = $assignedResource.attr('data-person-id')
                            //var attendanceOccurrenceId = $(target).closest('.js-scheduled-occurrence').find('.js-attendanceoccurrence-id').val();
                            //$.ajax({
                            //    method: "PUT",
                            //    url: scheduledPersonAssignUrl + '?personId=' + personId + '&attendanceOccurrenceId=' + attendanceOccurrenceId
                            //}).done(function (scheduledAttendance) {
                            //    // after adding a resource, repopulate the list of resources for the occurrence
                            //    var $occurrence = $(el).closest('.js-scheduled-occurrence');
                            //    self.populateScheduledOccurrence($occurrence);
                            //}).fail(function (a, b, c) {
                            //    // TODO
                            //    debugger
                            //})
                        }
                        self.trimSourceContainer();
                    });

                this.trimSourceContainer();
                this.initializeEventHandlers();

                var occurrenceEls = $(".js-mobile-blocktype-zone", $control).toArray();
                $.each(occurrenceEls, function (i) {
                    var $occurrence = $(occurrenceEls[i]);
                    self.populateScheduledOccurrence($occurrence);
                });
            },
            /** trims the source container if it just has whitespace, so that the :empty css selector works */
            trimSourceContainer: function () {
                // if js-mobile-blocktype-source-container just has whitespace in it, trim it so that the :empty css selector works
                var $sourceContainer = $('.js-mobile-blocktype-source-container');
                if (($.trim($sourceContainer.html()) == "")) {
                    $sourceContainer.html("");
                }
            },
            /** populates the blocks assinged to zones div */
            populateBlockTypesToZones: function ($occurrence) {

                var self = this;

            },

            initializeEventHandlers: function () {
                var self = this;

            }
        };

        return exports;
    }());
}(jQuery));







