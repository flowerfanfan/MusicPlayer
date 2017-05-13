using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using System.Xml.Linq;
using Windows.UI.Notifications;
using MusicPlayer.Models;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Xaml.Media.Imaging;

namespace MusicPlayer.Tile
{
    class TileManager
    {
        public static void ShowTile()
        {
            TileContent defaultContent = new TileContent()
            {
                Visual = new TileVisual()
                {
                    DisplayName = "Music Player",
                    Branding = TileBranding.Name,

                    TileSmall = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveText()
                    {
                        Text = "Title"
                    }
                }
                        }
                    },

                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveText()
                    {
                        Text = "Title"
                    },
                    new AdaptiveText()
                    {
                        Text = "Artist",
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    }
                }
                        }
                    },

                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveText()
                    {
                        Text = "Title"
                    },
                    new AdaptiveText()
                    {
                        Text = "Artist",
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    },
                    new AdaptiveText()
                    {
                        Text = "Album",
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    }
                }
                        }
                    },

                    TileLarge = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveText()
                    {
                        Text = "Title"
                    },
                    new AdaptiveText()
                    {
                        Text = "Artist",
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    },
                    new AdaptiveText()
                    {
                        Text = "Album",
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    }
                }
                        }
                    }
                }
            };

            TileNotification notification = new TileNotification(defaultContent.GetXml());
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        public static void UpdateTile(Song song)
        {
            string title = song.Title;
            string artist = song.Artist;
            string album = song.Album;
            BitmapImage cover = song.Cover;
            
            TileContent updateContent = new TileContent()
            {
                Visual = new TileVisual()
                {
                    DisplayName = "Music Player",
                    Branding = TileBranding.Name,

                    TileSmall = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveText()
                    {
                        Text = title
                    }
                }
                        }
                    },

                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveText()
                    {
                        Text = title
                    },
                    new AdaptiveText()
                    {
                        Text = artist,
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    }
                }
                        }
                    },

                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveText()
                    {
                        Text = title
                    },
                    new AdaptiveText()
                    {
                        Text = artist,
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    },
                    new AdaptiveText()
                    {
                        Text = album,
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    }
                }
                        }
                    },

                    TileLarge = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveText()
                    {
                        Text = title
                    },
                    new AdaptiveText()
                    {
                        Text = artist,
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    },
                    new AdaptiveText()
                    {
                        Text = album,
                        HintStyle = AdaptiveTextStyle.CaptionSubtle
                    }
                }
                        }
                    }
                }
            };

            TileNotification notification = new TileNotification(updateContent.GetXml());
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }
    }
}
