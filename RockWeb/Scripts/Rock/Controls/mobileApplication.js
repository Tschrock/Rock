(function ($) {
    'use strict';
    window.Rock = window.Rock || {};
    Rock.controls = Rock.controls || {};


    /** JS helper for the groupScheduler */
    Rock.controls.mobileApplication = (function () {
        var exports = {
            /** initializes the javascript for the groupScheduler */
            pageId: 0,
            zoneContainers: {},
            componentTemplate: {},
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
                var sourceClass = '.js-mobile-blocktype-source-container .js-drag-container';
                var targetClass = '.js-mobile-blocktype-target-container .js-drag-container';
                // add the resource list as a dragular container
                containers.push($control.find(sourceClass)[0]);

                // add all the occurrences (locations) as dragula containers
                var targets = $control.find(targetClass).toArray();
                $.each(targets, function (i) {
                    containers.push(targets[i]);
                });

                self.resourceListDrake = dragula(containers, {
                    moves: function (el, source, handle, sibling) {
                        return $(el).hasClass('component');
                    },
                    isContainer: function (el) {
                        return false;
                    },
                    copy: function (el, source) {
                        return sourceClass;
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
                            self.resourceScroll.disable();
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
                        if (target && target.classList.contains('js-drag-container')) {

                            var sourceParent = source.parentElement;
                            if (!sourceParent.classList.contains('js-mobile-blocktype-source-container')) {
                                //dropped from a different zone
                                //remove from current source;
                             
                                var sourceBlocks = $(source).find('.component');
                                if (sourceBlocks) {

                                    $(sourceBlocks).each(function (i, e) {
                                        if (e.firstElementChild.id == el.firstElementChild.id) {
                                            e.remove();
                                        }       
                                    });
                                }
                            }

                            var name = el.firstElementChild.innerText;
                            var $droppedElement = $(el.firstElementChild);
                            var blocktypeGuid = $droppedElement.attr('data-blocktype-guid');
                            var pageId = $droppedElement.attr('data-page-id');
                            self.pageId = pageId;
                            var blockId = $droppedElement.attr('data-blockId');
                            var zone = target.parentElement.firstElementChild.innerText;
                            var assingnBlockToZoneUrl = Rock.settings.get('baseUrl') + 'api/blocks/AssociateBlockToZone';

                            $.ajax({
                                method: "PUT",
                                url: assingnBlockToZoneUrl + '?blockId=' + blockId + '&name=' + name + '&pageId=' + pageId + '&blocktypeGuid=' + blocktypeGuid + '&zone=' + zone,
                            }).done(function (data) {
                                if (data) {
                                    // update drag item with block id
                                    $droppedElement.attr('data-blockId', data.Id);
                                    //after drag repopulate the zones
                                    self.populateBlockTypesToZones(self.pageId);
                                }

                            }).fail(function (a, b, c) {
                                // TODO
                            })
                        }
                        else {
                           
                        }
                        self.trimSourceContainer();
                    });

                this.trimSourceContainer();
                this.initializeEventHandlers();

                var occurrenceEls = $(".js-mobile-blocktype-zone", $control).toArray();
                $.each(occurrenceEls, function (i) {
                    var $occurrence = $(occurrenceEls[i]);
                    //self.populateScheduledOccurrence($occurrence);
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
            populateBlockTypesToZones: function (pageId) {
                var self = this;
                var getAssociateBlocksToZoneUrl = Rock.settings.get('baseUrl') + 'api/blocks/GetBlocksToZones';

                var $zoneContainers = $('.js-mobile-blocktype-target-container');
                self.zoneContainers = $zoneContainers;

                $.get(getAssociateBlocksToZoneUrl + '?pageId=' + pageId, function (blocks) {
                    $.each(blocks, function (i, e) {
                        var containers = self.zoneContainers;
                        var block = (e);
                        var currentZone = e.Zone;
                        if (currentZone) {
                            $(containers).each(function (i, e) {

                                var containerElement = e;
                                if (containerElement.firstElementChild.innerText == currentZone) {
                                    self.populateResourceDiv($(containerElement), block);
                                };
                            });
                        }
                    });
                });
            },

            populateResourceDiv: function ($container, block) {

                var resourceTemplate = $('.js-unassigned-block-resource-template').find('.component').clone();
                var resourceDiv = resourceTemplate.children(0);
                resourceDiv.attr('data-blocktype-guid', block.blocktypeGuid);
                resourceDiv.attr('data-page-id', block.pageId);
                resourceDiv.attr('data-blockid', block.Id);
                resourceDiv.innerText = block.Name;

                var currentContainer = $container.find('.js-drag-container');

                if (currentContainer) {
                    var currentChildren = currentContainer.find('.component');
                    if (currentChildren == null) {
                        //no children this is the first one
                        $(currentContainer).append(resourceDiv);

                    }
                    else {
                        $(currentChildren).filter(function (i, e) {
                            //Determine if the child is already there before adding
 
                        });
                    }
                }
            },

            initializeEventHandlers: function () {
                var self = this;


            }
        };

        return exports;
    }());
}(jQuery));







