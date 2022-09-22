import { Paper, Typography, useTheme } from '@mui/material';
import React from 'react';
import { FC, useEffect, useRef } from 'react';
import { createRoot } from 'react-dom/client';
import Globe, { GlobeMethods } from 'react-globe.gl';
import { funcWith3Types, funcWithType } from '../util';
import 'flag-icons/css/flag-icons.min.css';
import { useAppDispatch, useAppSelector } from '../store/Hooks';
import { Coords, Country, fetchCountries, Polygon, selectCountry } from '../store/World';
import { shallowEqual } from 'react-redux';

const RGlobe: FC = () => {
  const theme = useTheme();
  const dispatch = useAppDispatch();
  const globeRef = useRef<GlobeMethods>();

  const settings = useAppSelector(state => state.world.settings, shallowEqual);
  const countries = useAppSelector(state =>
    state.world.countries.map(country =>
      Object.create(country) as Country),
    shallowEqual);
  const selected = useAppSelector(state =>
    state.world.selected.map(country =>
      Object.create(country) as Country),
    shallowEqual);
  const focus = useAppSelector(state =>
    state.world.focus,
    shallowEqual);

  useEffect(() => {
    dispatch(fetchCountries());

    //if (globeRef.current && countries.length > 0) {
    //  globeRef.current.pointOfView({
    //    lng: (countries[0].geometry.mid[0] + countries[50].geometry.mid[0]) * 0.5,
    //    lat: (countries[0].geometry.mid[1] + countries[50].geometry.mid[1]) * 0.5,
    //    altitude: 1
    //  }, 10000);
    //} else if (globeRef.current) {
    //  globeRef.current.pointOfView({
    //    altitude: 4
    //  }, 0);
    //}
  }, []);

  useEffect(() => {
    if (focus) {
      globeRef.current!.pointOfView(focus, 1500);
    }
	}, [focus])

  useEffect(() => {
    if (settings) {
      (globeRef.current!.controls as any).enabled = settings.orbitControl;
      (globeRef.current!.controls as any).enablePan = settings.orbitControl;
      (globeRef.current!.controls as any).enableRotate = settings.orbitControl;
      
      (globeRef.current!.controls() as any).autoRotate = settings.orbitAutoRotate;
      (globeRef.current!.controls() as any).autoRotateSpeed = 0.5;
      globeRef.current!.pointOfView({ altitude: settings.zoom }, 2000);

      console.log("Settings.Zoom: ", settings.zoom);

      (globeRef.current!.scene() as THREE.Scene).visible = settings.show;
    }

    const onWindowResize = () => {
      const renderer = (globeRef.current!.renderer() as THREE.Renderer);
      const camera = (globeRef.current!.camera() as THREE.PerspectiveCamera);

      camera.aspect = window.innerWidth / window.innerHeight;
      camera.updateProjectionMatrix();
      renderer.setSize(window.innerWidth, window.innerHeight);
		}

    window.addEventListener('resize', onWindowResize, false);

    return () => {
      window.removeEventListener('resize', onWindowResize);
		}
  }, [settings.orbitControl, settings.zoom, settings.show])

  const handleCountrySelect = (country: Country, _1: MouseEvent, _2: Coords) => {
    dispatch(selectCountry({ isoCode: country.properties.ISO_A2}));
  }


  return (
      <Globe
        ref={globeRef}
        globeImageUrl="//unpkg.com/three-globe/example/img/earth-dark.jpg"
        backgroundColor={'rgba(0, 0, 0, 0)'}
        enablePointerInteraction={settings.interactable}
        polygonsData={countries}
        polygonAltitude={0.01}
        polygonCapColor={funcWithType<Country, string>(data =>
          selected.some(country => country.properties.NAME === data.properties.NAME)
            ? theme.palette.primary[theme.palette.mode]
            : 'rgba(0, 0, 0, 0)'
        )}
        polygonSideColor={() => 'rgba(0, 0, 0, 0)'}
        polygonStrokeColor={() => theme.palette.primary[theme.palette.mode]}
        polygonsTransitionDuration={0}
        onPolygonClick={funcWith3Types<Polygon, MouseEvent, Coords, void>(handleCountrySelect)}
        arcsData={countries.length > 0
          ? [{
            startLng: countries[0].geometry.mid[0],
            startLat: countries[0].geometry.mid[1],
            endLng: countries[50].geometry.mid[0],
            endLat: countries[50].geometry.mid[1],
            color: 'white',
            label: `${countries[0].properties.NAME} vs ${countries[99].properties.NAME}`
          }]
          : []}
        arcColor={'color'}
        arcLabel={'label'}
        arcStroke={2.5}
        arcsTransitionDuration={0}
        htmlElementsData={selected.map(country => ({
          lng: country.geometry.mid[0],
          lat: country.geometry.mid[1],
          altitude: 0.5,
          properties: country.properties
			  }))}
        //htmlElementsData={countries.length > 0
        //  ? [{
        //    lng: (countries[0].geometry.mid[0] + countries[50].geometry.mid[0]) * 0.5,
        //    lat: (countries[0].geometry.mid[1] + countries[50].geometry.mid[1]) * 0.5,
        //    altitude: ((countries[0].geometry.mid[0] - countries[50].geometry.mid[0]) * (countries[0].geometry.mid[0] - countries[50].geometry.mid[0])
        //      + (countries[0].geometry.mid[1] - countries[50].geometry.mid[1]) * (countries[0].geometry.mid[1] - countries[50].geometry.mid[1])),
        //    properties: countries[0].properties
        //  }]
        //  : []}
        htmlElement={funcWithType<Polygon, HTMLElement> (data => {
          const container = document.createElement('div');

          createRoot(container).render((
            <Paper
              sx={{
                mb: '100%',
                p: 1,
                textAlign: 'center',
                backgroundColor: theme.palette.neutral[theme.palette.mode]
						  }}
            >
              <span
                className={`fi fi-${data.properties.ISO_A2.toLowerCase()}`}
                style={{
                  display: 'inline-block',
                  fontSize: 36,
                  width: '100%'
							  }}
              />
              <Typography
                variant="h6"
                sx={{
                  color: theme.palette.text.primary
							  }}
              >
                {data.properties.NAME}
              </Typography>
              <Typography>
              </Typography>
            </Paper>
          ));

          return container
          })
        }
      />
    )
}

export default RGlobe;