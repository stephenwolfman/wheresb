if(location.href.toLowerCase().indexOf("bthedifference.org") > 0 && location.href.toLowerCase().indexOf("www.") <0)
{
    location.href = location.href.toLowerCase().replace("bthedifference.org","www.bthedifference.org");

}


        $(document).ready(function () {
            var wB = new WheresB();
            wB.initializeMap();
        });

var WheresB = function(){
    this.bMap = null;
    this.bJson = null;
    this.bIcon = 'http://www.bthedifference.org/images/BTheDiff_Logo_32.png';
};

WheresB.prototype = {


            initializeMap: function () {
            //var lat = 39.63398;
            //var long = -106.521015;-48.867187;
            var lat = 34.977201;
            var long = 10;
            
                    var myOptions = {
          center: new google.maps.LatLng(lat,long),
          zoom: 2,
          mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        bMap = new google.maps.Map(document.getElementById("map_canvas"),
            myOptions);
            bIcon = 'http://www.bthedifference.org/images/BTheDiff_Logo_32.png';
            var wB= this;
            setTimeout(function(){wB.loadMarkers();},1000);
        },
        
        loadMarkers: function(){
            var mUrl = "api/MapImages";
            //var mUrl = "wheresb.json";
            
            $.getJSON(mUrl,
                function(data){
                    
                    var bJson = data;
                    
                    var bounds = bMap.getBounds();
                    
                    var swLat = bounds.getSouthWest().lat();
                    var swLng = bounds.getSouthWest().lng();
                    var neLat = bounds.getNorthEast().lat();
                    var neLng = bounds.getNorthEast().lng();
                    
                    $(bJson).each(function(){

                        var lat = this.Lat;
                        var long = this.Long;
                        var inRange = true;
                        var title = this.Desc;
                        var imgUrl = this.ImageUrl;
                        var comment = this.Comment;
                        var sentBy = this.SentBy;
                        var vidUrl = this.VideoURL;
                        var contentS = '';
                        var openInfoWindow;
                        //Check the range of the map - let's do this when we have enough
                        
                        if(imgUrl != null || imgUrl == '')
                        {
                            contentS = '<div style="color:#1C573A; width:250px;"><div style="text-align:center;">' + title + '</div>' + '<div><img src="' + imgUrl.replace('../','http://www.bthedifference.org/') + '" style="width:240px;"/></div>' + '<div style="font-size:0.8em;">' + comment + '</div>' + '<div style="font-size:0.6em;">Taken by ' + sentBy + '</div></div>';
                        }
                        else{
                            contentS = '<div style="color:#1C573A; width:640px;"><div style="text-align:center;">' + title + '</div>' + '<iframe width="640" height="360" src="' + vidUrl + '" frameborder="0" allowfullscreen></iframe>' + '<div style="font-size:0.8em;">' + comment + '</div>' + '<div style="font-size:0.6em;">Taken by ' + sentBy + '</div></div>';
                        }
                        
                        if(inRange){
                            //Add Marker
                            var myLatlng = new google.maps.LatLng(lat,long);

                            var marker = new google.maps.Marker({
                                position: myLatlng,
                                map: bMap,
                                title:title,
                                icon:bIcon
                            });

                            var infowindow = null;
                            var myVar = '123';
                            //Add Overlay
                            google.maps.event.addListener(marker, 'click', function() {
                                _gaq.push(['_trackEvent', 'WheresB:Map', 'click', marker.title]);
                                if(openInfoWindow != null){
                                    openInfoWindow.close();
                                }
                                infowindow = new google.maps.InfoWindow({
                                    content: contentS
                                });
                                infowindow.open(bMap,marker);
                                openInfoWindow = infowindow;
                            });
                        }
                    });
                });
        }
};
