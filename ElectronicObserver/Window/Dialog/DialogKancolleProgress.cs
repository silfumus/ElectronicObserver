using ElectronicObserver.Data;
using ElectronicObserver.Resource;
using ElectronicObserver.Utility.Data;
using ElectronicObserver.Window.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElectronicObserver.Window.Dialog
{
    public partial class DialogKancolleProgress : Form
    {
        // base ship ID, level
        Dictionary<int, int> ShipLevels = new Dictionary<int, int>();

        List<DataGridViewCell> BorderTopCells = new List<DataGridViewCell>();
        List<DataGridViewCell> BorderBottomCells = new List<DataGridViewCell>();
        List<DataGridViewCell> BorderLeftCells = new List<DataGridViewCell>();
        List<DataGridViewCell> BorderRightCells = new List<DataGridViewCell>();

        public DialogKancolleProgress()
        {
            InitializeComponent();
            this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)ResourceManager.IconContent.FormEquipmentList]);

            GenerateList();
            ColorShipNames();

            DataGridViewCellStyle CS = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(38, 38, 38),
                ForeColor = Color.White,
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                SelectionBackColor = Color.FromArgb(76, 76, 76),
                Font = new Font("Meiryo UI", 12F, GraphicsUnit.Pixel)
            };

            ShipList.DefaultCellStyle = CS;
            ShipList.CellBorderStyle = DataGridViewCellBorderStyle.None;
            ShipList.AllowUserToResizeRows = false;
            ShipList.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            ShipList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            ShipList.BackgroundColor = Color.FromArgb(38, 38, 38);
            ShipList.GridColor = Color.FromArgb(38, 38, 38);

            ShipList.CellPainting += dataGridView1_CellPainting;

            ControlHelper.SetDoubleBuffered(ShipList);

            // DataGridView size
            ClientSize = new Size(1200, 550);
        }

        private void AddStatistics()
        {
            int ShipCount = ShipLevels.Count(x => x.Value >= 0);

            ShipList.Rows[3].Cells[21].Value = "Missing: " + ShipLevels.Count(x => x.Value == 0) + "/" + ShipCount;
            ShipList.Rows[3].Cells[21].Style.ForeColor = GetLevelColor(0);
            ShipList.Rows[3].Cells[21].Style.SelectionForeColor = GetLevelColor(0);

            ShipList.Rows[2].Cells[21].Value = "Collection: " + ShipLevels.Count(x => x.Value > 0) + "/" + ShipCount;
            ShipList.Rows[2].Cells[21].Style.ForeColor = GetLevelColor(1);
            ShipList.Rows[2].Cells[21].Style.SelectionForeColor = GetLevelColor(1);

            ShipList.Rows[4].Cells[21].Value = "Lv. 90+: " + ShipLevels.Count(x => x.Value >= 90) + "/" + ShipCount;
            ShipList.Rows[4].Cells[21].Style.ForeColor = GetLevelColor(90);
            ShipList.Rows[4].Cells[21].Style.SelectionForeColor = GetLevelColor(90);

            ShipList.Rows[5].Cells[21].Value = "Lv. 99+: " + ShipLevels.Count(x => x.Value >= 99) + "/" + ShipCount;
            ShipList.Rows[5].Cells[21].Style.ForeColor = GetLevelColor(99);
            ShipList.Rows[5].Cells[21].Style.SelectionForeColor = GetLevelColor(99);

            ShipList.Rows[6].Cells[21].Value = "Lv. Max: " + ShipLevels.Count(x => x.Value == 175) + "/" + ShipCount;
            ShipList.Rows[6].Cells[21].Style.ForeColor = GetLevelColor(175);
            ShipList.Rows[6].Cells[21].Style.SelectionForeColor = GetLevelColor(175);
        }

        private void GenerateList()
        {
            var Ships = KCDatabase.Instance.MasterShips.Values;
            Ships = Ships.Where(x => x.ShipID < 1500 && x.RemodelBeforeShipID == 0).OrderBy(x => x.SortID);

            var Destroyer = Ships.Where(x => x.ShipType == ShipTypes.Destroyer);
            var Escort = Ships.Where(x => x.ShipType == ShipTypes.Escort);
            var LightCruiser = Ships.Where(x => x.ShipType == ShipTypes.LightCruiser);
            var HeavyCruiser = Ships.Where(x => x.ShipType == ShipTypes.HeavyCruiser);
            var Battleship = Ships.Where(x => x.ShipType == ShipTypes.Battleship || x.ShipType == ShipTypes.Battlecruiser).OrderBy(x => (int) x.ShipType);
            var Carrier = Ships.Where(x => x.ShipType == ShipTypes.AircraftCarrier || x.ShipType == ShipTypes.LightAircraftCarrier
            || x.ShipType == ShipTypes.ArmoredAircraftCarrier).OrderBy(x => (int)x.ShipType);
            var Others = Ships.Where(x => x.ShipType == ShipTypes.Submarine || x.ShipType == ShipTypes.SubmarineAircraftCarrier
            || x.ShipType == ShipTypes.SeaplaneTender || x.ShipType == ShipTypes.FleetOiler || x.ShipType == ShipTypes.RepairShip
            || x.ShipType == ShipTypes.TrainingCruiser || x.ShipType == ShipTypes.AmphibiousAssaultShip || x.ShipType == ShipTypes.SubmarineTender)
            .OrderByDescending(x => (int)x.ShipType);

            var GroupedShips = new List<IEnumerable<ShipDataMaster>> { Destroyer, Escort, LightCruiser, HeavyCruiser, Battleship, Carrier, Others };

            int NumberOfRows = 30;
            ShipList.Rows.Add(NumberOfRows);

            ShipList.Rows[1].Cells[5].Value = Constants.GetShipClassClassification(ShipTypes.Destroyer);
            ShipList.Rows[1].Cells[5].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ShipList.Rows[1].Cells[9].Value = Constants.GetShipClassClassification(ShipTypes.Escort); ;
            ShipList.Rows[1].Cells[9].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ShipList.Rows[1].Cells[11].Value = Constants.GetShipClassClassification(ShipTypes.LightCruiser); ;
            ShipList.Rows[1].Cells[11].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ShipList.Rows[1].Cells[13].Value = Constants.GetShipClassClassification(ShipTypes.HeavyCruiser); ;
            ShipList.Rows[1].Cells[13].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ShipList.Rows[1].Cells[15].Value = Constants.GetShipClassClassification(ShipTypes.Battleship); ;
            ShipList.Rows[1].Cells[15].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ShipList.Rows[1].Cells[17].Value = Constants.GetShipClassClassification(ShipTypes.AircraftCarrier); ;
            ShipList.Rows[1].Cells[17].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ShipList.Rows[1].Cells[19].Value = "Others";
            ShipList.Rows[1].Cells[19].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            int PreviousShipClass = 0;

            foreach (var Ship in Ships)
                ShipLevels.Add(Ship.ShipID, 0);

            GetShipLevels();

            int MaxRowCount = 29;

            int RowCount = 2;
            int ColumnCount = 1;

            foreach (var Group in GroupedShips)
            {
                foreach (var Ship in Group)
                {
                    if (Ship.Name == "なし") continue;

                    if (Ship.ShipClass != PreviousShipClass)
                    {
                        BorderTopCells.Add(ShipList.Rows[RowCount].Cells[ColumnCount]);
                        BorderTopCells.Add(ShipList.Rows[RowCount].Cells[ColumnCount + 1]);

                        BorderBottomCells.Add(ShipList.Rows[RowCount - 1].Cells[ColumnCount]);
                        BorderBottomCells.Add(ShipList.Rows[RowCount - 1].Cells[ColumnCount + 1]);

                        if (RowCount + Group.Count(x => x.ShipClass == Ship.ShipClass) > MaxRowCount)
                        {
                            RowCount = 2;
                            ColumnCount += 2;
                        }

                        PreviousShipClass = Ship.ShipClass;
                    }

                    ShipList.Rows[RowCount].Cells[ColumnCount].Value = Ship.Name;
                    BorderLeftCells.Add(ShipList.Rows[RowCount].Cells[ColumnCount]);

                    ShipList.Rows[RowCount].Cells[ColumnCount + 1].Value = ShipLevels[Ship.ShipID];
                    ShipList.Rows[RowCount].Cells[ColumnCount + 1].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                    BorderRightCells.Add(ShipList.Rows[RowCount].Cells[ColumnCount + 1]);

                    RowCount++;

                    if (RowCount > MaxRowCount)
                    {
                        BorderTopCells.Add(ShipList.Rows[RowCount].Cells[ColumnCount]);
                        BorderTopCells.Add(ShipList.Rows[RowCount].Cells[ColumnCount + 1]);

                        RowCount = 2;
                        ColumnCount += 2;
                    }
                }
                BorderTopCells.Add(ShipList.Rows[RowCount].Cells[ColumnCount]);
                BorderTopCells.Add(ShipList.Rows[RowCount].Cells[ColumnCount + 1]);

                RowCount = 2;
                ColumnCount += 2;
            }

            AddStatistics();
        }

        private void ColorShipNames()
        {
            for (int i = 1; i < ShipList.ColumnCount - 1; i += 2)
                for (int j = 2; j < ShipList.RowCount; j++)
                {
                    Color c = GetLevelColor(ShipList.Rows[j].Cells[i + 1].Value);
                    ShipList.Rows[j].Cells[i].Style.ForeColor = c;
                    ShipList.Rows[j].Cells[i].Style.SelectionForeColor = c;
                }
        }

        private Color GetLevelColor(object Value)
        {
            if (Value == null)
                return Color.Black;

            int Level = 0;

            int.TryParse(Value.ToString(), out Level);

            if (Level == 175)
                return Color.FromArgb(255, 51, 153);

            if (Level >= 99)
                return Color.FromArgb(0, 176, 240);

            if (Level >= 90)
                return Color.FromArgb(0, 176, 80);

            if (Level >= 1)
                return Color.FromArgb(255, 255, 0);

            return Color.FromArgb(255, 0, 0);
        }

        private void GetShipLevels()
        {
            foreach (KeyValuePair<int, ShipData> Entry in KCDatabase.Instance.Ships)
            {
                int BaseID = BaseShipId(Entry.Value.ShipID);
                if (ShipLevels.ContainsKey(BaseID))
                {
                    if (ShipLevels[BaseID] < Entry.Value.Level)
                        ShipLevels[BaseID] = Entry.Value.Level;
                }
            }
        }

        private int BaseShipId(int ShipID)
        {
            ShipDataMaster Ship = KCDatabase.Instance.MasterShips[ShipID];
            while (Ship.RemodelBeforeShipID != 0)
                Ship = Ship.RemodelBeforeShip;
            return Ship.ID;
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            e.Handled = true;

            using (Brush b = new SolidBrush(ShipList.DefaultCellStyle.BackColor))
            {
                e.Graphics.FillRectangle(b, e.CellBounds);
            }

            using (Pen p = new Pen(Brushes.Black, 2))
            {
                int NumberOfColumns = 20;
                if (e.RowIndex == 1 && e.ColumnIndex >= 1 && e.ColumnIndex <= NumberOfColumns)
                {
                    BorderTop(p, e);
                    BorderBottom(p, e);

                    if (e.ColumnIndex == 1)
                        BorderLeft(p, e);

                    int NumberOfGroupedColumns = 8;
                    if (e.ColumnIndex >= NumberOfGroupedColumns && e.ColumnIndex % 2 == 0)
                        BorderRight(p, e);
                }

                if (BorderTopCells.Contains(ShipList.Rows[e.RowIndex].Cells[e.ColumnIndex]))
                    BorderTop(p, e);

                if (BorderBottomCells.Contains(ShipList.Rows[e.RowIndex].Cells[e.ColumnIndex]))
                    BorderBottom(p, e);

                if (BorderLeftCells.Contains(ShipList.Rows[e.RowIndex].Cells[e.ColumnIndex]))
                    BorderLeft(p, e);

                if (BorderRightCells.Contains(ShipList.Rows[e.RowIndex].Cells[e.ColumnIndex]))
                    BorderRight(p, e);
            }

            e.PaintContent(e.ClipBounds);
        }

        private void BorderTop(Pen p, DataGridViewCellPaintingEventArgs e)
        {
            e.Graphics.DrawLine(p, e.CellBounds.Left - 1, e.CellBounds.Top - 1, e.CellBounds.Right, e.CellBounds.Top - 1);
        }

        private void BorderBottom(Pen p, DataGridViewCellPaintingEventArgs e)
        {
            e.Graphics.DrawLine(p, e.CellBounds.Left - 1, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
        }

        private void BorderLeft(Pen p, DataGridViewCellPaintingEventArgs e)
        {
            e.Graphics.DrawLine(p, e.CellBounds.Left - 1, e.CellBounds.Top - 1, e.CellBounds.Left - 1, e.CellBounds.Bottom - 1);
        }

        private void BorderRight(Pen p, DataGridViewCellPaintingEventArgs e)
        {
            e.Graphics.DrawLine(p, e.CellBounds.Right - 1, e.CellBounds.Top - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
        }
    }
}