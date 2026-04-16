using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using Microvision.Types;

namespace Microvision.Scanners
{
    public class WiaDialogs : Citizen
    {
        // ***************************************************************************************************
        // 20.02.13 : création
        // 19.09.16 : ShowSelectItemsL à liste.
        // 12.05.17 : (libs 2.1)
        // 21.11.19 : (libs 2.2)
        // 14.04.22 : (libs 3.0)
        // ***************************************************************************************************

        private WIA.CommonDialog _dialogs;


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public WiaDialogs() : base()
        {
            _dialogs = new WIA.CommonDialog();
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public WiaImageFile ShowAcquireImage()
        {
            WIA.ImageFile imgf = _dialogs.ShowAcquireImage(WIA.WiaDeviceType.ScannerDeviceType, FormatID: WiaItem.wiaFormatBmp);

            WiaImageFile output = null;
            if (imgf is not null) output = new WiaImageFile(imgf);

            return output;
        }

        public object ShowAcquisitionWizard(WiaDevice dev)
        {
            return _dialogs.ShowAcquisitionWizard(dev.Core);
        }

        public void ShowDeviceProperties(WiaDevice dev)
        {
            _dialogs.ShowDeviceProperties(dev.Core);
        }

        public void ShowItemProperties(WiaItem itm)
        {
            _dialogs.ShowItemProperties(itm.Core);
        }

        public WiaDevice ShowSelectDevice(WiaDevice.DeviceType devtyp)
        {
            WIA.Device dev = _dialogs.ShowSelectDevice((WIA.WiaDeviceType)devtyp, false, false);

            WiaDevice output = null;
            if (dev is not null) output = new WiaDevice(dev);

            return output;
        }

        public List<WiaItem> ShowSelectItems(WiaDevice dev)
        {
            List<WIA.Item> itms = _dialogs.ShowSelectItems(dev.Core).ToList();

            List<WiaItem> output = null;
            if (itms is not null) output = itms.Select(o => new WiaItem(o)).ToList();

            return output;
        }

        public WiaImageFile ShowTransfer(WiaItem itm, string fmtid)
        {
            WIA.ImageFile imgf = (WIA.ImageFile)_dialogs.ShowTransfer(itm.Core, fmtid);

            WiaImageFile output = null;
            if (imgf is not null) output = new WiaImageFile(imgf);

            return output;
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            if (_dialogs is not null)
            {
                Marshal.ReleaseComObject(_dialogs);
                _dialogs = null;
            }

            base.oDispose(isExplicit);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------


        // ----------------------------------------
        // Evènements
        // ----------------------------------------


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}