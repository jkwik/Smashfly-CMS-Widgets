var map = null;
var pushpins = null;
var clusterLayer = null;
var Latitude = document.currentScript.getAttribute("Latitude");
var Longitude = document.currentScript.getAttribute("Longitude");

var JobPushpinBase64 = document.currentScript.getAttribute("JobPushpinBase64");
var JobPushpinHeight = document.currentScript.getAttribute("JobPushpinHeight");
var JobPushpinWidth = document.currentScript.getAttribute("JobPushpinWidth");

var UserPushpinBase64 = document.currentScript.getAttribute(
  "UserPushpinBase64"
);
var UserPushpinHeight = document.currentScript.getAttribute(
  "UserPushpinHeight"
);
var UserPushpinWidth = document.currentScript.getAttribute("UserPushpinWidth");

function LoadJobMap() {
  pushpins = [];

  map = new Microsoft.Maps.Map("#jobMap", {
    credentials: "BING MAP API KEY",
    zoom: 5
  });

  //Request the user's location
  navigator.geolocation.getCurrentPosition(function(position) {
    var userLocation = new Microsoft.Maps.Location(
      position.coords.latitude,
      position.coords.longitude
    );

    createScaledUserPushpin(UserPushpinBase64, function(userImage, userAnchor) {
      var userImageData = null;
      var userAnchorLocation = null;

      userImageData = userImage;
      userAnchorLocation = userAnchor;

      var userpushpinoptions = {};

      if (UserPushpinBase64 !== "empty") {
        userpushpinoptions.icon = userImageData;
        userpushpinoptions.anchor = userAnchorLocation;
      }

      //Add a pushpin at the user's location.
      var userPin = new Microsoft.Maps.Pushpin(
        userLocation,
        userpushpinoptions
      );
      pushpins.push(userPin);
      map.entities.push(userPin);

      var jobLocation = new Microsoft.Maps.Location(Latitude, Longitude);

      createScaledJobPushpin(JobPushpinBase64, function(jobImage, jobAnchor) {
        var jobImageData = null;
        var jobAnchorLocation = null;

        jobImageData = jobImage;
        jobAnchorLocation = jobAnchor;

        var jobpushpinoptions = {};

        if (JobPushpinBase64 !== "empty") {
          jobpushpinoptions.icon = jobImageData;
          jobpushpinoptions.anchor = jobAnchorLocation;
        }
        //Add a pushpin at the user's location.
        var jobPin = new Microsoft.Maps.Pushpin(jobLocation, jobpushpinoptions);
        pushpins.push(jobPin);
        map.entities.push(jobPin);

        map.setView({
          bounds: Microsoft.Maps.LocationRect.fromShapes(pushpins)
        });
      });
    });
  });
}

function createScaledJobPushpin(image, callback) {
  var img = new Image();
  img.onload = function() {
    var c = document.createElement("canvas");
    c.width = JobPushpinWidth;
    c.height = JobPushpinHeight;

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

function createScaledUserPushpin(image, callback) {
  var img = new Image();
  img.onload = function() {
    var c = document.createElement("canvas");
    c.width = UserPushpinWidth;
    c.height = UserPushpinHeight;

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
