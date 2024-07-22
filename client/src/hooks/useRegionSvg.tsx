import { SVGProps } from 'react';
import ADR from '../assets/map/ADR.svg?react';
import AEG from '../assets/map/AEG.svg?react';
import Alb from '../assets/map/Alb.svg?react';
import Ank from '../assets/map/Ank.svg?react';
import Apu from '../assets/map/Apu.svg?react';
import Arm from '../assets/map/Arm.svg?react';
import BAL from '../assets/map/BAL.svg?react';
import BAR from '../assets/map/BAR.svg?react';
import Bel from '../assets/map/Bel.svg?react';
import Ber from '../assets/map/Ber.svg?react';
import BLA from '../assets/map/BLA.svg?react';
import Boh from '../assets/map/Boh.svg?react';
import BOT from '../assets/map/BOT.svg?react';
import Bre from '../assets/map/Bre.svg?react';
import Bud from '../assets/map/Bud.svg?react';
import BulE from '../assets/map/Bul_E.svg?react';
import BulS from '../assets/map/Bul_S.svg?react';
import Bur from '../assets/map/Bur.svg?react';
import Cly from '../assets/map/Cly.svg?react';
// Can't name a file 'Con' on Windows...thanks Tom Scott...
import Con from '../assets/map/Con_.svg?react';
import Den from '../assets/map/Den.svg?react';
import EAS from '../assets/map/EAS.svg?react';
import Edi from '../assets/map/Edi.svg?react';
import ENG from '../assets/map/ENG.svg?react';
import Fin from '../assets/map/Fin.svg?react';
import Gal from '../assets/map/Gal.svg?react';
import Gas from '../assets/map/Gas.svg?react';
import Gre from '../assets/map/Gre.svg?react';
import HEL from '../assets/map/HEL.svg?react';
import Hol from '../assets/map/Hol.svg?react';
import ION from '../assets/map/ION.svg?react';
import IRI from '../assets/map/IRI.svg?react';
import Kie from '../assets/map/Kie.svg?react';
import Lon from '../assets/map/Lon.svg?react';
import Lvn from '../assets/map/Lvn.svg?react';
import Lvp from '../assets/map/Lvp.svg?react';
import LYO from '../assets/map/LYO.svg?react';
import MAO from '../assets/map/MAO.svg?react';
import Mar from '../assets/map/Mar.svg?react';
import Mos from '../assets/map/Mos.svg?react';
import Mun from '../assets/map/Mun.svg?react';
import Naf from '../assets/map/Naf.svg?react';
import NAO from '../assets/map/NAO.svg?react';
import Nap from '../assets/map/Nap.svg?react';
import NTH from '../assets/map/NTH.svg?react';
import NWG from '../assets/map/NWG.svg?react';
import Nwy from '../assets/map/Nwy.svg?react';
import Par from '../assets/map/Par.svg?react';
import Pic from '../assets/map/Pic.svg?react';
import Pie from '../assets/map/Pie.svg?react';
import Por from '../assets/map/Por.svg?react';
import Pru from '../assets/map/Pru.svg?react';
import Rom from '../assets/map/Rom.svg?react';
import Ruh from '../assets/map/Ruh.svg?react';
import Rum from '../assets/map/Rum.svg?react';
import Ser from '../assets/map/Ser.svg?react';
import Sev from '../assets/map/Sev.svg?react';
import Sil from '../assets/map/Sil.svg?react';
import SKA from '../assets/map/SKA.svg?react';
import Smy from '../assets/map/Smy.svg?react';
import SpaN from '../assets/map/Spa_N.svg?react';
import SpaS from '../assets/map/Spa_S.svg?react';
import StpN from '../assets/map/Stp_N.svg?react';
import StpS from '../assets/map/Stp_S.svg?react';
import Swe from '../assets/map/Swe.svg?react';
import Syr from '../assets/map/Syr.svg?react';
import Tri from '../assets/map/Tri.svg?react';
import Tun from '../assets/map/Tun.svg?react';
import Tus from '../assets/map/Tus.svg?react';
import Tyr from '../assets/map/Tyr.svg?react';
import TYS from '../assets/map/TYS.svg?react';
import Ukr from '../assets/map/Ukr.svg?react';
import Ven from '../assets/map/Ven.svg?react';
import Vie from '../assets/map/Vie.svg?react';
import Wal from '../assets/map/Wal.svg?react';
import War from '../assets/map/War.svg?react';
import WES from '../assets/map/WES.svg?react';
import Yor from '../assets/map/Yor.svg?react';

// TODO add license for use
const useRegionSvg = (id: string) =>
  ({
    ADR,
    AEG,
    Alb,
    Ank,
    Apu,
    Arm,
    BAL,
    BAR,
    Bel,
    Ber,
    BLA,
    Boh,
    BOT,
    Bre,
    Bud,
    Bul_E: BulE,
    Bul_S: BulS,
    Bul: (props: SVGProps<SVGSVGElement>) => (
      <>
        <BulE {...props} />
        <BulS {...props} />
      </>
    ),
    Bur,
    Cly,
    Con,
    Den,
    EAS,
    Edi,
    ENG,
    Fin,
    Gal,
    Gas,
    Gre,
    HEL,
    Hol,
    ION,
    IRI,
    Kie,
    Lon,
    Lvn,
    Lvp,
    LYO,
    MAO,
    Mar,
    Mos,
    Mun,
    Naf,
    NAO,
    Nap,
    NTH,
    NWG,
    Nwy,
    Par,
    Pic,
    Pie,
    Por,
    Pru,
    Rom,
    Ruh,
    Rum,
    Ser,
    Sev,
    Sil,
    SKA,
    Smy,
    Spa_N: SpaN,
    Spa_S: SpaS,
    Spa: (props: SVGProps<SVGSVGElement>) => (
      <>
        <SpaN {...props} />
        <SpaS {...props} />
      </>
    ),
    Stp_N: StpN,
    Stp_S: StpS,
    Stp: (props: SVGProps<SVGSVGElement>) => (
      <>
        <StpN {...props} />
        <StpS {...props} />
      </>
    ),
    Swe,
    Syr,
    Tri,
    Tun,
    Tus,
    Tyr,
    TYS,
    Ukr,
    Ven,
    Vie,
    Wal,
    War,
    WES,
    Yor,
  })[id];

export default useRegionSvg;
