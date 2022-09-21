import MUI, { createTheme, PaletteColorOptions } from '@mui/material/styles';
import { green, grey, pink, purple } from '@mui/material/colors';
import { LinkProps, Palette, PaletteMode, PaletteOptions } from '@mui/material';
import React from 'react';
import { Link, LinkProps as RouterLinkProps } from 'react-router-dom';

declare module '@mui/material/styles' {
  interface Theme {
    status: {
      danger: React.CSSProperties['color'];
    };
  }

  interface Palette {
    mode: PaletteMode;
    primary: Palette['primary'];
    secondary: Palette['secondary'];
    neutral: Palette['secondary'];
  }
  interface PaletteOptions {
    mode?: PaletteMode;
    primary?: PaletteColorOptions;
    secondary?: PaletteColorOptions;
    neutral?: PaletteColorOptions;
  }

  interface PaletteColor {
    darker?: string;
  }
  interface SimplePaletteColorOptions {
    darker?: string;
  }
}


const LinkBehavior = React.forwardRef<
  HTMLAnchorElement,
  Omit<RouterLinkProps, 'to'> & { href: RouterLinkProps['to'] }
>((props, ref) => {
  const { href, ...other } = props;
  // Map href (MUI) -> to (react-router)
  return <Link ref={ref} to={href} {...other} />;
});

export default (mode: PaletteMode) => createTheme({
  palette: {
    mode: mode,
    primary: {
      main: '#f06292',
    },
    secondary: {
      main: '#18ffff',
    },
    neutral: {
      light: '#ffffff',
      main: '#ffffff',
      dark: '#121212'
		}
  },
  components: {
    MuiLink: {
      defaultProps: {
        component: LinkBehavior,
      } as LinkProps,
    },
    MuiButtonBase: {
      defaultProps: {
        LinkComponent: LinkBehavior,
      },
    },
    MuiListItemButton: {
      defaultProps: {
        LinkComponent: LinkBehavior
			}
		}
  }
});