using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaCenter;

namespace TwitchBot
{
    class MCHelper
    {
        static MCAutomation mcAuto;

        MCHelper()
        {
            mcAuto = new MCAutomation();
        }

        struct MediaFile
        {
            public String Name
            {
                get { return mcAuto.GetCurPlaylist().GetFile(mcAuto.GetCurPlaylist().Position).Name; }
            }
            public String Artist
            {
                get { return mcAuto.GetCurPlaylist().GetFile(mcAuto.GetCurPlaylist().Position).Artist; }
            }
            public String Album
            {
                get { return mcAuto.GetCurPlaylist().GetFile(mcAuto.GetCurPlaylist().Position).Album; }
            }
            public String Track
            {
                get { return mcAuto.GetCurPlaylist().GetFile(mcAuto.GetCurPlaylist().Position).Tracknumber.ToString(); }
            }
            public String Pos
            {
                get
                {
                    TimeSpan ts = new TimeSpan(0, 0, mcAuto.GetPlayback().Position);
                    return ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
                }
            }
            public String Dur
            {
                get
                {
                    TimeSpan tt;
                    tt = new TimeSpan(0, 0, mcAuto.GetPlayback().Duration);
                    return tt.Minutes + ":" + tt.Seconds;
                }
            }
        }

        public MediaFile CurrentTrack
        {
            get
            {
                if (mcAuto.GetPlayback().State == MJPlaybackStates.PLAYSTATE_PLAYING)
                {
                    return new MediaFile();
                }
                else
                {
                    MediaFile blank = new MediaFile();

                }
            }
        }
    }
}
