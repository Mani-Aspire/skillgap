using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using iConnect.Common;
using iConnect.Entities;

namespace iConnect.DataAccess
{
    public class MeetingRoomDataAccess
    {
		private IRepository<MeetingRoom> _repository;
		public MeetingRoomDataAccess()
		{
		}
		public MeetingRoomDataAccess(IRepository<MeetingRoom> repository)
		{
			_repository = repository;
		}
        public Collection<MeetingRoomEntity> SearchMeetingRoom(string searchText, int pageIndex,int pageSize)
        {
            using(IRepository<MeetingRoom> meetingRoomRepository = new Repository<MeetingRoom,SkillGapEntities>(new SkillGapEntities()))
            {
                ISpecification<MeetingRoom> meetingRoomSpec = new Specification<MeetingRoom>();
                meetingRoomSpec.Predicate = room => room.RoomName.Contains(searchText);
				meetingRoomSpec.Predicate = meetingRoomSpec.OrExpression( meetingRoomSpec.Predicate, room => room.Floor.Contains( searchText ) );
				meetingRoomSpec.Predicate = meetingRoomSpec.OrExpression( meetingRoomSpec.Predicate, room => room.Phase.Contains( searchText ) );
                IList<MeetingRoom> data = meetingRoomRepository.SelectAll(meetingRoomSpec, null, "RoomName", "ASC", pageSize, (pageIndex - 1)*
                                                                                                    pageSize);
                return Common.Mapper<MeetingRoom, MeetingRoomEntity>.Convert(data);
            }
        }        
    }
}
