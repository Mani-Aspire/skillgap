ICNamespace( "IConnect.AnnounOperations", function( self ) {
	var announcement = ICNamespace( "IConnect.Announcement" );
	var utils = ICNamespace( "IConnect.Utils" );

	return {
		LoadAnnouncement: function( callback ) {
			//Assume data comes from the Service
			var returnObj = {
				Title: "Announcements xxxx"
			}
			callback( returnObj );
		}
	}
} );