var geoSearch;
var pushpins;
var JobsApiUrl = document.currentScript.getAttribute("JobsApiUrl");
var PushpinBase64 = document.currentScript.getAttribute("PushpinBase64");
var PushpinHeight = document.currentScript.getAttribute("PushpinHeight");
var PushpinWidth = document.currentScript.getAttribute("PushpinWidth");
var ClusterRadius = document.currentScript.getAttribute("ClusterRadius");
var ClusterColor = document.currentScript.getAttribute("ClusterColor");
var ClusterTextColor = document.currentScript.getAttribute("ClusterTextColor");
var DistanceUnit = document.currentScript.getAttribute("DistanceUnit");
var MinutesUnit = document.currentScript.getAttribute("MinutesUnit");
var HoursUnit = document.currentScript.getAttribute("HoursUnit");
var PolygonFillColor = document.currentScript.getAttribute("PolygonFillColor");
var PolygonStrokeColor = document.currentScript.getAttribute(
  "PolygonStrokeColor"
);
var RadiusSearch = document.currentScript.getAttribute("RadiusSearch");
var TravelTimeSearch = document.currentScript.getAttribute("TravelTimeSearch");
var PolygonSearch = document.currentScript.getAttribute("PolygonSearch");
var ClusterDiameter = ClusterRadius * 2;
var ChoseSelection = false;
var map = null;
var selectedSuggestion = null;
var prevPushpin = null;
var prevRadius = null;
var prevPolygon = null;
var clusterLayer = null;
var searchSelection = null;

function buildStateCityCascadingDropdowns(url) {
  if (!$("#states").length || !$("#cities").length) {
    buildNonCascadingDropDowns(url);
    return;
  }

  var states = $("#states").kendoDropDownList({
    height: "auto",
    cascade: function() {
      $("#cities")
        .data("kendoDropDownList")
        .dataSource.read();
    },
    dataSource: new kendo.data.DataSource({
      transport: {
        read: {
          url: function() {
            return url;
          },
          dataType: "json",
          data: function() {
            return {
              sort: null,
              group: "State-asc",
              filter: "(Country~gt~''~and~State~gt~''~and~City~gt~'')",
              fields: null,
              pageSize: 2147483647,
              groupingCountsOnly: true
            };
          }
        }
      },
      schema: {
        parse: function(response) {
          var states = [];

          if (response.Data.length < 1) {
            return states;
          }

          for (var i = 0; i < response.Data.length; i++) {
            states.push({
              State: response.Data[i].Items[0].State,
              Total: response.Data[i].Items[0].Total
            });
          }

          return states;
        }
      }
    }),
    dataTextField: "State",
    enable: true,
    autoBind: true,
    dataValueField: "State",
    optionLabel: "Select state..."
  });

  var cities = $("#cities").kendoDropDownList({
    height: "auto",
    dataSource: new kendo.data.DataSource({
      transport: {
        read: {
          url: function() {
            return url;
          },
          dataType: "json",
          data: function() {
            return {
              sort: null,
              group: "State-asc~City-asc",
              filter: kendo.format(
                "(State~eq~'{0}'~and~City~gt~'')",
                $("#states").val()
              ),
              fields: null,
              pageSize: 2147483647,
              groupingCountsOnly: true,
              maxAge: 60
            };
          }
        }
      },
      schema: {
        parse: function(response) {
          var cities = [];

          if (response.Data.length < 1) {
            return cities;
          }

          for (var i = 0; i < response.Data[0].Items.length; i++) {
            cities.push({
              State: response.Data[0].Items[i].State,
              City: response.Data[0].Items[i].City,
              Total: response.Data[0].Items[i].Total
            });
          }

          return cities.sort(function(a, b) {
            return a.City > b.City;
          });
        }
      }
    }),
    autoBind: false,
    enable: false,
    dataTextField: "City",
    dataValueField: "City",
    optionLabel: "Select city...",
    cascadeFrom: "states"
  });
}

function DrawPolygon(location, map) {
  if (prevPolygon != null) {
    map.entities.remove(prevPolygon);
  }

  if (prevPushpin != null || prevRadius != null) {
    map.entities.remove(prevPushpin);
    map.entities.remove(prevRadius);
  }

  Microsoft.Maps.loadModule(
    ["Microsoft.Maps.SpatialDataService", "Microsoft.Maps.Search"],
    function() {
      var searchManager = new Microsoft.Maps.Search.SearchManager(map);
      var geocodeRequest = {
        where: location,
        callback: function(geocodeResult) {
          if (
            geocodeResult &&
            geocodeResult.results &&
            geocodeResult.results.length > 0
          ) {
            map.setView({ bounds: geocodeResult.results[0].bestView });
            var geoDataRequestOptions = {
              entityType: "PopulatedPlace",
              getAllPolygons: true
            };

            //Add opacity of 40% to selected color (66).
            var polygonFillColor = PolygonFillColor + "66";

            var polygonOptions = {
              fillColor: polygonFillColor,
              strokeColor: PolygonStrokeColor,
              strokeThickness: 1
            };

            Microsoft.Maps.SpatialDataService.GeoDataAPIManager.getBoundary(
              geocodeResult.results[0].location,
              geoDataRequestOptions,
              map,
              function(data) {
                if (data.results && data.results.length > 0) {
                  prevPolygon = data.results[0].Polygons;
                  map.entities.push(prevPolygon);
                }
              },
              polygonOptions,
              function errCallback(networkStatus, statusMessage) {
                console.log(networkStatus);
                console.log(statusMessage);
              }
            );
          }
        }
      };
      searchManager.geocode(geocodeRequest);
    }
  );
}

function DrawRadius(location, map, RadiusSize, ShowCircle) {
  if (prevPushpin != null || prevRadius != null) {
    map.entities.remove(prevPushpin);
    map.entities.remove(prevRadius);
  }

  if (prevPolygon != null) {
    map.entities.remove(prevPolygon);
  }

  var backgroundColor = new Microsoft.Maps.Color(10, 100, 0, 0);
  var borderColor = new Microsoft.Maps.Color(150, 200, 0, 0);
  var earthRadius = 3959; //Earth mean radius in miles
  var MM = Microsoft.Maps;

  var circleLat = location.latitude;
  var circleLon = location.longitude;

  var lat = (circleLat * Math.PI) / 180;
  var lon = (circleLon * Math.PI) / 180;
  var diameter = parseFloat(RadiusSize) / earthRadius;

  var circlePoints = new Array();

  for (x = 0; x <= 365; x += 5) {
    var p2 = new MM.Location(0, 0);
    brng = (x * Math.PI) / 180;
    p2.latitude = Math.asin(
      Math.sin(lat) * Math.cos(diameter) +
        Math.cos(lat) * Math.sin(diameter) * Math.cos(brng)
    );
    p2.longitude =
      ((lon +
        Math.atan2(
          Math.sin(brng) * Math.sin(diameter) * Math.cos(lat),
          Math.cos(diameter) - Math.sin(lat) * Math.sin(p2.latitude)
        )) *
        180) /
      Math.PI;

    p2.latitude = (p2.latitude * 180) / Math.PI;
    circlePoints.push(p2);
  }

  prevRadius = new MM.Polygon(circlePoints, {
    fillColor: backgroundColor,
    strokeColor: borderColor,
    strokeThickness: 1
  });

  if (ShowCircle) {
    map.entities.push(prevRadius);
  }

  prevPushpin = new Microsoft.Maps.Pushpin(location, { color: "green" });
  map.entities.push(prevPushpin);

  pushpins.push(prevPushpin);

  if (searchSelection == "travel") {
    map.setView({
      bounds: Microsoft.Maps.LocationRect.fromShapes(pushpins)
    });
  } else {
    var boundary = Microsoft.Maps.LocationRect.fromLocations(circlePoints);
    map.setView({
      bounds: boundary
    });
  }
}

function GetRadiusMap() {
  map = new Microsoft.Maps.Map("#myRadiusMap", {
    credentials: "BING MAP API KEY",
    zoom: 5
  });

  Microsoft.Maps.loadModule("Microsoft.Maps.AutoSuggest", function() {
    var manager = new Microsoft.Maps.AutosuggestManager({
      map: map
    });
    manager.attachAutosuggest(
      "#searchBox",
      "#searchBoxContainer",
      suggestionSelected
    );
  });

  function suggestionSelected(result) {
    ChoseSelection = true;
    selectedSuggestion = result;
  }

  //Load the Bing Spatial Data Services module
  Microsoft.Maps.loadModule(
    [
      "Microsoft.Maps.Clustering",
      "Microsoft.Maps.SpatialDataService",
      "Microsoft.Maps.Search"
    ],
    function() {
      var searchManager = new Microsoft.Maps.Search.SearchManager(map);
      var geocodeRequest = {
        where: "United States",
        callback: function(geocodeResult) {
          if (
            geocodeResult &&
            geocodeResult.results &&
            geocodeResult.results.length > 0
          ) {
            var geoDataRequestOptions = {
              entityType: "CountryRegion",
              getAllPolygons: false
            };

            Microsoft.Maps.SpatialDataService.GeoDataAPIManager.getBoundary(
              geocodeResult.results[0].location,
              geoDataRequestOptions,
              map,
              function(data) {
                if (data.results && data.results.length > 0) {
                  geoSearch = new GeoSearch(JobsApiUrl, function(e) {
                    //Clear the cluster layer.
                    if (clusterLayer != null) {
                      clusterLayer.clear();
                    }

                    if (searchSelection == null) {
                      map.entities.clear();
                    }

                    pushpins = [];

                    var data = e.response.Data;

                    var imageData = null;
                    var anchorLocation = null;
                    createScaledPushpin(PushpinBase64, function(image, anchor) {
                      imageData = image;
                      anchorLocation = anchor;

                      var pushpinoptions = {};

                      if (PushpinBase64 !== "empty") {
                        pushpinoptions.icon = imageData;
                        pushpinoptions.anchor = anchorLocation;
                      }

                      for (var j = 0; j < data.length; j++) {
                        var location = new Microsoft.Maps.Location(
                          data[j].Latitude,
                          data[j].Longitude
                        );
                        //var pushpinoptions = {
                        //    title: data[j].JobTitle
                        //}
                        pushpinoptions.title = data[j].JobTitle;
                        var pushpin = new Microsoft.Maps.Pushpin(
                          location,
                          pushpinoptions
                        );
                        Microsoft.Maps.Events.addHandler(
                          pushpin,
                          "click",
                          pushpinClicked
                        );

                        if (data[j].UrlJobTitle.charAt(0) == ".") {
                          data[j].UrlJobTitle = data[j].UrlJobTitle.substring(
                            1,
                            data[j].UrlJobTitle.length
                          );
                        }

                        pushpin.metadata = {
                          category: data[j].ShortTextField1,
                          location: data[j].City + ", " + data[j].State,
                          ApplyUrl: data[j].ApplyTrackingUrl,
                          JobDetails:
                            "PATH TO JOB DETAILS PAGE" +
                            data[j].JobId +
                            "/" +
                            data[j].UrlJobTitle,
                          title: data[j].JobTitle
                        };
                        pushpins.push(pushpin);
                      }

                      //CLUSTERS
                      //Create a ClusterLayer with options and add it to the map.
                      var clusterOptions = {
                        clusteredPinCallback: function(cluster) {
                          var svg = [
                            '<svg xmlns="http://www.w3.org/2000/svg" width="',
                            ClusterDiameter,
                            '" height="',
                            ClusterDiameter,
                            '">',
                            '<circle cx="',
                            ClusterRadius,
                            '" cy="',
                            ClusterRadius,
                            '" r="',
                            ClusterRadius,
                            '" fill="',
                            ClusterColor,
                            '"/>',
                            '<text x="50%" y="50%" stroke="',
                            ClusterTextColor,
                            '" text-anchor="middle" stroke-width="1px" dy=".3em" >{text}</text>',
                            "</svg>"
                          ];
                          cluster.setOptions({
                            icon: svg.join(""),
                            text: cluster.containedPushpins.length,
                            anchor: new Microsoft.Maps.Point(
                              ClusterRadius,
                              ClusterRadius
                            )
                          });
                        }
                      };
                      clusterLayer = new Microsoft.Maps.ClusterLayer(
                        pushpins,
                        clusterOptions
                      );
                      //Add a click event to the clusterLayer
                      Microsoft.Maps.Events.addHandler(
                        clusterLayer,
                        "click",
                        clusterClicked
                      );
                      map.layers.insert(clusterLayer);
                      if (searchSelection == null) {
                        map.setView({
                          bounds: Microsoft.Maps.LocationRect.fromShapes(
                            pushpins
                          )
                        });
                      } else if (searchSelection == "travel") {
                        if (ChoseSelection) {
                          DrawRadius(
                            selectedSuggestion.bestView.center,
                            map,
                            0,
                            false
                          );
                        } else {
                          DrawRadiusWithManualAddress(false, 0);
                        }
                      }
                    });

                    //INFOBOX
                    function showInfobox() {
                      var infobox = new Microsoft.Maps.Infobox(
                        pushpins[0].getLocation(),
                        {
                          visible: false
                        }
                      );
                      infobox.setMap(map);
                      for (var i = 0; i < pushpins.length; i++) {
                        var pushpin = pushpins[i];
                        //Store some metadata with the pushpin
                        pushpin.metadata = {
                          pushpinNumber: i,
                          JobID: data[i].DisplayJobId,
                          ApplyUrl: data[i].ApplyTrackingUrl,
                          City: data[i].City,
                          State: data[i].State,
                          JobDetails:
                            "PATH TO JOB DETAILS PAGE" +
                            data[i].JobId +
                            "/" +
                            data[i].UrlJobTitle
                        };

                        Microsoft.Maps.Events.addHandler(
                          pushpin,
                          "click",
                          function(args) {
                            infobox.setOptions({
                              location: args.target.getLocation(),
                              title: args.target.entity.title,
                              description: args.target.metadata.pushpinNumber,
                              visible: true,
                              actions: [
                                {
                                  label: "View Details",
                                  eventHandler: function(e) {
                                    window.open(
                                      args.target.metadata.JobDetails
                                    );
                                  }
                                },
                                {
                                  label: "Apply",
                                  eventHandler: function(e) {
                                    window.open(args.target.metadata.ApplyUrl);
                                  }
                                }
                              ]
                            });
                          }
                        );
                      }
                    }
                  });

                  function clusterClicked(e) {
                    //Check to see if the clicked pushpin has any contained pushpins (this would be a cluster).
                    if (e.target.containedPushpins) {
                      var locs = [];
                      var targetData = [];
                      var joblist = "";

                      var job = "";
                      len = 0;
                      for (
                        var i = 0, len = e.target.containedPushpins.length;
                        i < len;
                        i++
                      ) {
                        //Get the location of each pushpin.
                        var place = e.target.containedPushpins[i].getLocation();
                        var jobData = e.target.containedPushpins[i].metadata;
                        locs.push(place);
                        targetData.push(jobData);
                      }
                      newlocs = locs.filter(
                        (elem, index, self) =>
                          self.findIndex(t => {
                            return (
                              t.latitude === elem.latitude &&
                              t.longitude === elem.longitude
                            );
                          }) === index
                      );
                      if (newlocs.length === 1) {
                        var row = "";

                        var tableData = [];

                        for (var i = 0; i < targetData.length; i++) {
                          var row = {
                            title: targetData[i].title,
                            category: targetData[i].category,
                            location: targetData[i].location,
                            details: targetData[i].JobDetails,
                            apply: targetData[i].ApplyUrl
                          };
                          tableData.push(row);
                        }

                        var newDataSource = new kendo.data.DataSource({
                          data: tableData,
                          pageSize: 10
                        });

                        var jobGrid = $("#job-list").data("kendoGrid");
                        jobGrid.setDataSource(newDataSource);

                        $("#overlay").show();
                      } else {
                        //Create a bounding box for the pushpins.
                        var bounds = Microsoft.Maps.LocationRect.fromLocations(
                          locs
                        );
                        //Zoom into the bounding box of the cluster.
                        //Add padding to compensate for the pixel area of the pushpins.
                        map.setView({
                          bounds: bounds,
                          padding: 100
                        });
                      }
                    }
                  }

                  function pushpinClicked(e) {
                    var pushpinMetadata = e.target.metadata;
                    var tableData = [];
                    var row = {
                      title: pushpinMetadata.title,
                      category: pushpinMetadata.category,
                      location: pushpinMetadata.location,
                      details: pushpinMetadata.JobDetails,
                      apply: pushpinMetadata.ApplyUrl
                    };
                    tableData.push(row);
                    var newDataSource = new kendo.data.DataSource({
                      data: tableData
                    });

                    var jobGrid = $("#job-list").data("kendoGrid");
                    jobGrid.setDataSource(newDataSource);

                    $("#overlay").show();
                  }

                  //RADIUS SEARCH API CALL WITH RADIUS 5000 TO GET ALL JOBS.
                }
              },
              null,
              function errCallback(networkStatus, statusMessage) {
                console.log(networkStatus);
                console.log(statusMessage);
              }
            );
          }
        }
      };

      searchManager.geocode(geocodeRequest);
    }
  );
}

function createScaledPushpin(image, callback) {
  var img = new Image();
  img.onload = function() {
    var c = document.createElement("canvas");
    c.width = PushpinWidth;
    c.height = PushpinHeight;

    var context = c.getContext("2d");

    //Draw scaled image
    context.drawImage(img, 0, 0, c.width, c.height);

    if (callback) {
      callback(
        c.toDataURL(),
        new Microsoft.Maps.Point(c.width / 2, c.height / 2)
      );
    }
  };

  if (image !== "empty") {
    img.src = image;
  } else {
    if (callback) {
      callback(image, null);
    }
  }
}

function addressChanged(address) {
  ChoseSelection = false;
  selectedSuggestion = address;
}

function displayForm(choice) {
  $("#mapSearchButtons").css("display", "flex");
  $("#divMapPartialView").css("marginTop", "20px");

  if (choice == "radiusSearch") {
    $("#radiusSearchContainer").show();
    $("#radiusSearchContainer").prepend($("#searchBox"));
    $("#searchBoxContainer").show();
    $("#travelTimeSearchContainer").hide();
    $("#polygonSearchContainer").hide();
    $("#radiusTab").addClass("active");

    $("#travelTimeTab").removeClass("active");
    $("#polygonTab").removeClass("active");
    $("#errorDiv").hide();
    searchSelection = "radius";
  } else if (choice == "travelTimeSearch") {
    $("#radiusSearchContainer").hide();
    $("#travelTimeSearchContainer").show();
    $("#travelTimeSearchContainer").prepend($("#searchBox"));

    $("#searchBoxContainer").show();
    $("#polygonSearchContainer").hide();
    $("#travelTimeTab").addClass("active");

    $("#radiusTab").removeClass("active");
    $("#polygonTab").removeClass("active");
    $("#errorDiv").hide();
    searchSelection = "travel";
  } else if (choice == "polygonSearch") {
    $("#searchBoxContainer").hide();
    $("#radiusSearchContainer").hide();
    $("#travelTimeSearchContainer").hide();

    $("#polygonSearchContainer").css("display", "inline-block");
    $("#polygonTab").addClass("active");
    $("#travelTimeTab").removeClass("active");
    $("#radiusTab").removeClass("active");
    $("#errorDiv").hide();
    searchSelection = "polygon";
  }
}

function clearSearch() {
  $("#radiusTab").removeClass("active");
  $("#travelTimeTab").removeClass("active");
  $("#polygonTab").removeClass("active");
  $("#radiusSearchContainer").hide();
  $("#polygonSearchContainer").hide();
  $("#travelTimeSearchContainer").hide();
  $("#mapSearchButtons").hide();
  $("#divMapPartialView").css("marginTop", "60px");
  $("#errorDiv").hide();
  searchSelection = null;
  //RADIUS SEARCH WITH RADIUS 5000 TO GET ALL JOBS.
}

$(document).ready(function() {
  $("#mapFilter").click(function() {
    if (searchSelection == "radius") {
      //Ensure they specify an address.
      if (selectedSuggestion == null || selectedSuggestion == "") {
        $("#errorMessage").html("Please type an address and select an option");
        $("#errorDiv").css("display", "inline-block");
        return;
      }
      var radius = $("#radiusBox").val();

      if (radius == "") {
        $("#errorMessage").html("Please specify a radius");
        $("#errorDiv").css("display", "inline-block");
        return;
      }

      if (DistanceUnit === "kilometers") {
        radius *= 0.621371;
      }

      if (ChoseSelection) {
        //RADIUS SEARCH API CALL WITH RADIUS OF radius.
        DrawRadius(selectedSuggestion.bestView.center, map, radius, true);
      } else {
        //RADIUS SEARCH API CALL WITH RADIUS OF radius.
        DrawRadiusWithManualAddress(true, radius);
      }

      $("#errorDiv").hide();

      return;
    }

    if (searchSelection == "travel") {
      //Ensure they specify an address.
      if (selectedSuggestion == null || selectedSuggestion == "") {
        $("#errorMessage").html("Please type an address and select an option");
        $("#errorDiv").css("display", "inline-block");
        return;
      }

      var travelTimeTotal = 0;

      if (HoursUnit) {
        travelTimeTotal += $("#travelTimeBoxHours").val() * 60;
      }

      if (MinutesUnit) {
        travelTimeTotal += $("#travelTimeBoxMinutes").val();
      }

      var travelMode = $("#travelMethodBox").val();

      if (travelTimeTotal == 0) {
        $("#errorMessage").html("Please specify the travel time");
        $("#errorDiv").css("display", "inline-block");
        return;
      }

      if (travelMode == "") {
        $("#errorMessage").html("Please specify a travel mode");
        $("#errorDiv").css("display", "inline-block");
        return;
      }

      $("#errorDiv").hide();

      if (ChoseSelection) {
        //CALL TRAVEL TIME SEARCH API WITH selectedSuggestion.formattedSuggestion, travelTimeTotal and travelMode.
      } else {
        //CALL TRAVEL TIME SEARCH API WITH selectedSuggestion, travelTimeTotal and travelMode.
      }

      return;
    }

    if (searchSelection == "polygon") {
      var city = $("#cities").val();
      var state = $("#states").val();
      //Make sure they specify a city and state.

      if (state == "") {
        $("#errorMessage").html("Please specify a state");
        $("#errorDiv").css("display", "inline-block");
        return;
      }

      if (city == "") {
        $("#errorMessage").html("Please specify a city");
        $("#errorDiv").css("display", "inline-block");
        return;
      }

      $("#errorDiv").hide();

      //CALL POLYGON SEARCH API WITH city + "," + state.

      //Draw polygon based on city, state.
      DrawPolygon(city + "," + state, map);
      return;
    }
  });
});

$(document).ready(function() {
  $("#close-button").click(function() {
    $("#overlay").hide();
  });
});

function DrawRadiusWithManualAddress(showRadius, radius) {
  Microsoft.Maps.loadModule("Microsoft.Maps.Search", function() {
    var searchManager = new Microsoft.Maps.Search.SearchManager(map);
    var requestOptions = {
      bounds: map.getBounds(),
      where: selectedSuggestion,
      callback: function(answer, userData) {
        DrawRadius(answer.results[0].bestView.center, map, radius, showRadius);
      }
    };
    searchManager.geocode(requestOptions);
  });
}

//Initialize city and state drop down.
$(document).ready(function() {
  buildStateCityCascadingDropdowns(JobsApiUrl);
  var data = [
    { text: "Method of Travel", value: "" },
    { text: "Walking", value: "Walking" },
    { text: "Transit", value: "Transit" },
    { text: "Driving", value: "Driving" }
  ];
  $("#travelMethodBox").kendoDropDownList({
    dataTextField: "text",
    dataValueField: "value",
    dataSource: data
  });

  $("#job-list").kendoGrid({
    dataSource: [],
    columns: [
      {
        template: '<div><a href="#:details#">#:title#</a></div>',
        field: "title",
        title: "Title"
      },
      //{ template: '<div>#:category#</div>', field: "category", title: "Category" },
      {
        template: "<div>#:location#</div>",
        field: "location",
        title: "Location"
      },
      {
        template: '<div><a href="#:apply#">Apply</a></div>',
        field: "apply",
        title: "Apply"
      }
    ],
    pageable: {
      pageSizes: true
    }
  });

  if (!MinutesUnit) {
    $("#travelTimeBoxMinutes").hide();
  }

  if (!HoursUnit) {
    $("#travelTimeBoxHours").hide();
  }

  var radiusSearch = $("#radiusSearch");
  RadiusSearch
    ? radiusSearch.css("display", "inline-block")
    : radiusSearch.hide();

  var travelTimeSearch = $("#travelTimeSearch");
  TravelTimeSearch
    ? travelTimeSearch.css("display", "inline-block")
    : travelTimeSearch.hide();

  var polygonSearch = $("#polygonSearch");
  PolygonSearch
    ? polygonSearch.css("display", "inline-block")
    : polygonSearch.hide();
});
