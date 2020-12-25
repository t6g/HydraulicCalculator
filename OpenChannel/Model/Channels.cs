using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;

namespace OpenChannel.Model
{
    //to provide 
    public interface IModelOptions
    {
        bool bUSCustomary { get; }
    }
    public interface IViewOptions
    {
        bool bRatingCurve { get; }
        bool bEnergyCurve { get; }
        List<ManningsNLine> pManningsNTable { get; }
    }
    public class Channels : IModelOptions //, IViewOptions
    {
        public TriangularChannel Tria = null;
        public RectangularChannel Rect = null;
        public TrapezoidalChannel Trap = null;
        public CircularChannel Circ = null;
        public EllipticalChannel Elli = null;
        public ParabolicChannel Para = null;
        public ArchChannel Arch = null;
        public string Current { get; set; } = null;
        // model options
        private bool isUSCustomary;

        // view options
        private bool isRatingCurve;
        private bool isEnergyCurve;
        public bool IsUSCustomary { get => isUSCustomary; set { isUSCustomary = value; } }
        public bool IsRatingCurve { get => isRatingCurve; set { isRatingCurve = value; } }
        public bool IsEnergyCurve { get => isEnergyCurve; set { isEnergyCurve = value; } }

        public bool bUSCustomary => isUSCustomary;
        //public bool bRatingCurve => isRatingCurve;
        //public bool bEnergyCurve => isEnergyCurve;
        //[XmlIgnore]
        //public List<ManningsNLine> pManningsNTable => ManningsNTable;
        [XmlIgnore]
        public List<ManningsNLine> ManningsNTable { set; get; } = null;

        public Channels()
        {
            isUSCustomary = true;
            isRatingCurve = true;
            isEnergyCurve = true;

            if (Tria == null) Tria = new TriangularChannel(this);
            if (Rect == null) Rect = new RectangularChannel(this);
            if (Trap == null) Trap = new TrapezoidalChannel(this);
            if (Circ == null) Circ = new CircularChannel(this);
            if (Para == null) Para = new ParabolicChannel(this);
            if (Elli == null) Elli = new EllipticalChannel(this);
            if (Arch == null) Arch = new ArchChannel(this);
            Current = "Triangular";

            if (ManningsNTable == null)
            {
                ManningsNTable = new List<ManningsNLine>();
                Task.Run(() => ReadManningsNAsync()).Wait();
            }
        }
        //called when input file is read to create channels
        public void SetOptions()
        {
            if (Tria != null) if (Tria.options == null) Tria.options = this;
            if (Rect != null) if (Rect.options == null) Rect.options = this;
            if (Trap != null) if (Trap.options == null) Trap.options = this;
            if (Circ != null) if (Circ.options == null) Circ.options = this;
            if (Para != null) if (Para.options == null) Para.options = this;
            if (Elli != null) if (Elli.options == null) Elli.options = this;
            if (Arch != null) if (Arch.options == null) Arch.options = this;
        }

        public async Task ReadManningsNAsync()
        {
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile sampleFile = await storageFolder.GetFileAsync("Mannings.txt");
                string text = await FileIO.ReadTextAsync(sampleFile);
                using (StringReader sr = new StringReader(text))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] words = line.Split('|');
                        ManningsNLine man = new ManningsNLine(words);
                        ManningsNTable.Add(man);
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                //var dlg = new MessageDialog(ex.Message, "Open file failed!");
                //await dlg.ShowAsync();
            }
            catch (Exception ex)
            {
                //var dlg = new MessageDialog(ex.Message, "Open file failed!");
                //await dlg.ShowAsync();
            }

        }

    }
}
