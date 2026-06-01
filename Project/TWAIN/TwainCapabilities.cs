using System;
using System.Collections.Generic;

using Microvision.Geometry;
using Microvision.Types;

using TWAINWorkingGroup;

namespace Microvision.Scanners
{
    internal class TwainCapabilities : Citizen
    {
        // ***************************************************************************************************
        // 13.03.23 : Création
        // 23.04.24 : Ajout de la propriété HasGamma.
        // ***************************************************************************************************

        internal delegate void PhysicalSizeChangedEventHandler(float width, float height);

        internal event PhysicalSizeChangedEventHandler? PhysicalSizeChanged;

        // ***************************************************************************************************

        private const float KMMPerInch = 25.4f;


        private TWAIN _dsm;

        // Obligatoires :
        private readonly TwainCapability<short> _xferCount;                      // TWAIN 2.5 page 539/766:  TW_INT16, MSG_GETCURRENT:TW_ONEVALUE, MSG_SET:TW_ONEVALUE
        private readonly TwainCapabilityEnum<TWAIN.TWSX, ushort> _xferMech;      // TWAIN 2.5 page 637/766: TW_UINT16, MSG_GETCURRENT:TW_ONEVALUE, MSG_SET:TW_ONEVALUE
        private readonly TwainCapabilityEnum<TWAIN.TWUN, ushort> _units;         // TWAIN 2.5 page 636/766: TW_UINT16, MSG_GETCURRENT:TW_ONEVALUE, MSG_SET:TW_ONEVALUE
        private readonly TwainCapabilityEnum<TWAIN.TWCP, ushort> _compression;   // TWAIN 2.5 page 567/766: TW_UINT16, MSG_GETCURRENT:TW_ONEVALUE, MSG_SET:TW_ONEVALUE
        private readonly TwainCapabilityFloat _physicalHeight;                   // TWAIN 2.5 page 617/766:  TW_FIX32, MSG_GETCURRENT:TW_ONEVALUE, MSG_SET:Not allowed
        private readonly TwainCapabilityFloat _physicalWidth;                    // TWAIN 2.5 page 618/766:  TW_FIX32, MSG_GETCURRENT:TW_ONEVALUE, MSG_SET:Not allowed
        private readonly TwainCapabilityFloat _xResolution;                      // TWAIN 2.5 page 639/766:  TW_FIX32, MSG_GETCURRENT:TW_ONEVALUE, MSG_SET:TW_ONEVALUE, MSG_GET:TW_ONEVALUE|TW_RANGE|TW_ENUMERATION
        private readonly TwainCapabilityFloat _yResolution;                      // TWAIN 2.5 page 642/766:  TW_FIX32, MSG_GETCURRENT:TW_ONEVALUE, MSG_SET:TW_ONEVALUE, MSG_GET:TW_ONEVALUE|TW_RANGE|TW_ENUMERATION
        private readonly TwainCapabilityEnum<TWAIN.TWPT, ushort> _pixelType;     // TWAIN 2.5 page 621/766: TW_UINT16, MSG_GETCURRENT:TW_ONEVALUE, MSG_SET:TW_ONEVALUE
        private readonly TwainCapability<ushort> _bitDepth;                      // TWAIN 2.5 page 559/766: TW_UINT16, MSG_GETCURRENT:TW_ONEVALUE, MSG_SET:TW_ONEVALUE
        private readonly TwainCapabilityRectangleF _frame;                       // TWAIN 2.5 page 581/766:  TW_FRAME, MSG_GETCURRENT:TW_ONEVALUE, MSG_SET:TW_ONEVALUE, MSG_GET:TW_ONEVALUE|TW_ENUMERATION

        // Optionnels : 
        private readonly TwainCapabilityFloat? _gamma;                           // TWAIN 2.5 page 582/766:  TW_FIX32, MSG_GETCURRENT:TW_ONEVALUE, MSG_SET:TW_ONEVALUE, MSG_GET:TW_ONEVALUE|TW_RANGE, MSG_GETDEFAULT:TW_ONEVALUE
        private readonly TwainCapabilityEnum<TWAIN.TWLP, ushort>? _lightPath;    // TWAIN 2.5 page 600/766: TW_UINT16, MSG_GETCURRENT:TW_ONEVALUE, MSG_SET:TW_ONEVALUE, MSG_GET:TW_ONEVALUE|TW_ENUMERATION


        // ----------------------------------------
        // Classe
        // ----------------------------------------

        public TwainCapabilities(TWAIN dsm) : base()
        {
            _dsm = dsm;

            _xferCount = new TwainCapability<short>(_dsm, TWAIN.CAP.CAP_XFERCOUNT);
            _xferMech = new TwainCapabilityEnum<TWAIN.TWSX, ushort>(_dsm, TWAIN.CAP.ICAP_XFERMECH);
            _units = new TwainCapabilityEnum<TWAIN.TWUN, ushort>(_dsm, TWAIN.CAP.ICAP_UNITS);
            _compression = new TwainCapabilityEnum<TWAIN.TWCP, ushort>(_dsm, TWAIN.CAP.ICAP_COMPRESSION);
            _physicalHeight = new TwainCapabilityFloat(_dsm, TWAIN.CAP.ICAP_PHYSICALHEIGHT);
            _physicalWidth = new TwainCapabilityFloat(_dsm, TWAIN.CAP.ICAP_PHYSICALWIDTH);
            _xResolution = new TwainCapabilityFloat(_dsm, TWAIN.CAP.ICAP_XRESOLUTION);
            _yResolution = new TwainCapabilityFloat(_dsm, TWAIN.CAP.ICAP_YRESOLUTION);
            _pixelType = new TwainCapabilityEnum<TWAIN.TWPT, ushort>(_dsm, TWAIN.CAP.ICAP_PIXELTYPE);
            _bitDepth = new TwainCapability<ushort>(_dsm, TWAIN.CAP.ICAP_BITDEPTH);
            _frame = new TwainCapabilityRectangleF(_dsm, TWAIN.CAP.ICAP_FRAMES);

            if (zQuerySupport(dsm, TWAIN.CAP.ICAP_LIGHTPATH, (int)(TWAIN.TWQC.SET | TWAIN.TWQC.GETCURRENT | TWAIN.TWQC.GET)))
            {
                _lightPath = new TwainCapabilityEnum<TWAIN.TWLP, ushort>(_dsm, TWAIN.CAP.ICAP_LIGHTPATH);
                _lightPath_Attach(true);
            }

            if (zQuerySupport(_dsm, TWAIN.CAP.ICAP_GAMMA, (int)(TWAIN.MSG.SET | TWAIN.MSG.GETCURRENT | TWAIN.MSG.GETDEFAULT)))
            {
                _gamma = new TwainCapabilityFloat(_dsm, TWAIN.CAP.ICAP_GAMMA);
            }

            // cf. TWAIN 2.5 page 422/766 § Best Practices for Applications
            _xferCount.SetOneValue(1);
            _xferMech.SetOneValue(TWAIN.TWSX.MEMORY);
            _units.SetOneValue(TWAIN.TWUN.INCHES);  // les millimètres ne sont pas supportés par l'EPSON Perfection V850 Pro. Comme les pouces sont l'unité par défaut après ouverture (cf. ICAP_UNITS), ils sont obligatoirement supportés par tous les scanners.
            _compression.SetOneValue(TWAIN.TWCP.NONE);

            _pixelType.SetOneValue(TWAIN.TWPT.RGB);
            _bitDepth.SetOneValue(24);

            // Certains scanners autorisent plusieurs frames par page (ce n'est pas le cas de l'EPSON Perfection V850 Pro.)
            // Les lignes ci-dessous indiquent à ces scanners que l'application ne gère qu'une seule frame par page.
            if (zQuerySupport(_dsm, TWAIN.CAP.ICAP_MAXFRAMES, (int)(TWAIN.MSG.SET | TWAIN.MSG.GETCURRENT)))
            {
                TwainCapability<ushort> maxFrames = new TwainCapability<ushort>(_dsm, TWAIN.CAP.ICAP_MAXFRAMES);  // TWAIN 2.5 page 602/766: TW_UINT16, MSG_GETCURRENT:TW_ONEVALUE, MSG_SET:TW_ONEVALUE, MSG_GET:TW_ONEVALUE|TW_RANGE
                maxFrames.SetOneValue(1);
                maxFrames.Dispose();
            }

            // Les capabilities ICAP_XSCALING et ICAP_YSCALING influent sur la résolution réelle.
            // Pour éviter les problèmes, nous figeons ces capabilities à 1.
            // cf. § Resolution sur: https://www.epsondevelopers.com/twain-programming-guide-epson-scan/epson-twain-driver/
            if (zQuerySupport(_dsm, TWAIN.CAP.ICAP_XSCALING, (int)(TWAIN.MSG.SET | TWAIN.MSG.GETCURRENT)))
            {
                TwainCapabilityFloat xScaling = new TwainCapabilityFloat(_dsm, TWAIN.CAP.ICAP_XSCALING);
                xScaling.SetOneValue(1);
                xScaling.Dispose();
            }

            if (zQuerySupport(_dsm, TWAIN.CAP.ICAP_YSCALING, (int)(TWAIN.MSG.SET | TWAIN.MSG.GETCURRENT)))
            {
                TwainCapabilityFloat yScaling = new TwainCapabilityFloat(_dsm, TWAIN.CAP.ICAP_YSCALING);
                yScaling.SetOneValue(1);
                yScaling.Dispose();
            }
        }


        // ----------------------------------------
        // Propriétés
        // ----------------------------------------

        public bool HasGamma => _gamma is not null;

        public bool HasResolutionRange => _xResolution.IsGetRange && _yResolution.IsGetRange;

        public float PhysicalHeight => _physicalHeight.GetCurrentOneValue() * KMMPerInch;

        public float PhysicalWidth => _physicalWidth.GetCurrentOneValue() * KMMPerInch;


        // ----------------------------------------
        // Statiques
        // ----------------------------------------

        public static bool CheckSupportedCaps(TWAIN dsm)
        {
            bool ok = true;

            ok = ok && zQuerySupport(dsm, TWAIN.CAP.CAP_XFERCOUNT, (int)(TWAIN.TWQC.SET | TWAIN.TWQC.GETCURRENT));
            ok = ok && zQuerySupport(dsm, TWAIN.CAP.ICAP_XFERMECH, (int)(TWAIN.TWQC.SET | TWAIN.TWQC.GETCURRENT));
            ok = ok && zQuerySupport(dsm, TWAIN.CAP.ICAP_UNITS, (int)(TWAIN.TWQC.SET | TWAIN.TWQC.GETCURRENT));
            ok = ok && zQuerySupport(dsm, TWAIN.CAP.ICAP_COMPRESSION, (int)(TWAIN.TWQC.SET | TWAIN.TWQC.GETCURRENT));
            ok = ok && zQuerySupport(dsm, TWAIN.CAP.ICAP_PHYSICALHEIGHT, (int)TWAIN.TWQC.GETCURRENT);
            ok = ok && zQuerySupport(dsm, TWAIN.CAP.ICAP_PHYSICALWIDTH, (int)TWAIN.TWQC.GETCURRENT);
            ok = ok && zQuerySupport(dsm, TWAIN.CAP.ICAP_XRESOLUTION, (int)(TWAIN.TWQC.SET | TWAIN.TWQC.GETCURRENT | TWAIN.TWQC.GET));
            ok = ok && zQuerySupport(dsm, TWAIN.CAP.ICAP_YRESOLUTION, (int)(TWAIN.TWQC.SET | TWAIN.TWQC.GETCURRENT | TWAIN.TWQC.GET));
            ok = ok && zQuerySupport(dsm, TWAIN.CAP.ICAP_PIXELTYPE, (int)(TWAIN.TWQC.SET | TWAIN.TWQC.GETCURRENT | TWAIN.TWQC.GET));
            ok = ok && zQuerySupport(dsm, TWAIN.CAP.ICAP_BITDEPTH, (int)(TWAIN.TWQC.SET | TWAIN.TWQC.GETCURRENT));
            ok = ok && zQuerySupport(dsm, TWAIN.CAP.ICAP_FRAMES, (int)(TWAIN.TWQC.SET | TWAIN.TWQC.GETCURRENT));

            return ok;
        }


        // ----------------------------------------
        // Méthodes
        // ----------------------------------------

        public float GetDefaultGamma()
        {
            _gamma.ThrowIfNull();

            return _gamma.GetDefaultOneValue();
        }

        public RectG GetFrame()
        {
            RectG rect = _frame.GetCurrentOneValue();
            rect.X *= KMMPerInch;
            rect.Y *= KMMPerInch;
            rect.Width *= KMMPerInch;
            rect.Height *= KMMPerInch;

            return rect;
        }

        public TWAIN.TWPT GetPixelType()
        {
            return _pixelType.GetCurrentOneValue();
        }

        public (float resX, float resY) GetResolution()
        {
            float resX = _xResolution.GetCurrentOneValue();
            float resY = _yResolution.GetCurrentOneValue();

            return (resX, resY);
        }

        public (float minX, float maxX, float minY, float maxY) GetResolutionRange()
        {
            (float minX, float maxX, _, _, _) = _xResolution.GetRange();
            (float minY, float maxY, _, _, _) = _yResolution.GetRange();

            return (minX, maxX, minY, maxY);
        }

        public List<TWAIN.TWLP> GetSupportedLightPaths()
        {
            List<TWAIN.TWLP> lightPaths;

            if (_lightPath is not null)
                lightPaths = _lightPath.IsGetEnumeration ? _lightPath.GetEnumeration() : [_lightPath.GetOneValue()];
            else
                lightPaths = [];

            return lightPaths;
        }

        public List<TWAIN.TWPT> GetSupportedPixelTypes()
        {
            List<TWAIN.TWPT> pixelTypes = _pixelType.IsGetEnumeration ? _pixelType.GetEnumeration() : [_pixelType.GetOneValue()];

            return pixelTypes;
        }

        public void SetFrame(RectG frame)
        {
            frame.X /= KMMPerInch;
            frame.Y /= KMMPerInch;
            frame.Width /= KMMPerInch;
            frame.Height /= KMMPerInch;

            _frame.SetOneValue(frame);
        }

        public void SetGamma(float gamma, bool force = false)
        {
            _gamma.ThrowIfNull();

            _gamma.SetOneValue(gamma, force);
        }

        public void SetLightPath(TWAIN.TWLP lightPath)
        {
            _lightPath.ThrowIfNull();

            _lightPath.SetOneValue(lightPath);
        }

        public void SetPixelType(TWAIN.TWPT pixelType)
        {
            _pixelType.SetOneValue(pixelType);
        }

        public void SetResolution(float resX, float resY)
        {
            _xResolution.SetOneValue(resX);
            _yResolution.SetOneValue(resY);
        }


        // ----------------------------------------
        // Semi-privées
        // ----------------------------------------

        protected override void oDispose(bool isExplicit)
        {
            if (isExplicit) _xferCount.Dispose();

            if (isExplicit) _xferMech.Dispose();

            if (isExplicit) _units.Dispose();

            if (isExplicit) _compression.Dispose();

            if (isExplicit) _physicalHeight.Dispose();

            if (isExplicit) _physicalWidth.Dispose();

            if (isExplicit) _xResolution.Dispose();

            if (isExplicit) _yResolution.Dispose();

            if (isExplicit) _pixelType.Dispose();

            if (isExplicit) _bitDepth.Dispose();

            if (isExplicit) _frame.Dispose();

            if (_lightPath is not null)
            {
                _lightPath_Attach(false);
                if (isExplicit) _lightPath.Dispose();
            }

            if (_gamma is not null)
            {
                if (isExplicit) _gamma.Dispose();
            }

            base.oDispose(isExplicit);
        }


        // ----------------------------------------
        // Privées
        // ----------------------------------------

        private static bool zQuerySupport(TWAIN dsm, TWAIN.CAP cap, int neededOps)
        {
            bool ok = false;

            TWAIN.TW_CAPABILITY capability;
            capability.Cap = cap;
            capability.ConType = TWAIN.TWON.ONEVALUE;
            capability.hContainer = IntPtr.Zero;

            if (dsm.DatCapability(TWAIN.DG.CONTROL, TWAIN.MSG.QUERYSUPPORT, ref capability) == TWAIN.STS.SUCCESS)
            {
                int supportedOps = capability.GetOneValue<int>(dsm);
                ok = ((supportedOps & neededOps) == neededOps);

                dsm.DsmMemFree(ref capability.hContainer);
            }

            return ok;
        }


        // ----------------------------------------
        // Evènements
        // ----------------------------------------

        private void _lightPath_Attach(bool attach)
        {
            _lightPath.ThrowIfNull();

            if (attach)
            {
                _lightPath.ValueChanged += _lightPath_ValueChanged;
            }
            else
            {
                _lightPath.ValueChanged -= _lightPath_ValueChanged;
            }
        }

        private void _lightPath_ValueChanged()
        {
            float width = _physicalWidth.GetCurrentOneValue() * KMMPerInch;
            float height = _physicalHeight.GetCurrentOneValue() * KMMPerInch;

            this.PhysicalSizeChanged?.Invoke(width, height);
        }


        // ----------------------------------------
        // Implémentations
        // ----------------------------------------

    }
}