ICNamespace( "IConnect.Announcement", function( self ) {

	var announOperations = ICNamespace( "IConnect.AnnounOperations" );

	// Here's my data model
	var ViewModel = function( data ) {
		this.Title = ko.observable( data.Title );
	};

	function load( returnObj ) {
		announOperations.LoadAnnouncement( function( returnObj ) {
			ko.applyBindings( new ViewModel( returnObj ) );
		} );
	};

	return {
		Load: load
	}
} );