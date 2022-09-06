import { FC, useEffect, useRef } from 'react';
import * as THREE from 'three';
import ThreeGlobe from 'three-globe';

const TGlobe: FC = () => {
	const ref = useRef<THREE.Group>(null!);

	useEffect(() => {
		fetch('./ne_110m_admin_0_countries.geojson')
			.then(result => result.json())
			.then(countries => {
				const globe = new ThreeGlobe()
					.globeImageUrl('./img/worldColour.5400x2700.jpg')
					.bumpImageUrl('./img/srtm_ramp2.world.5400x2700.jpg')
					.polygonsData(countries.features.filter(d => d.properties.ISO_A2 !== 'AQ'))
					.polygonCapColor(() => 'rgba(200, 0, 0, 0.7)')
					//.polygonSideColor(() => 'rgba(0, 200, 0, 0.1)')
					//.polygonCapColor(() => 'rgba(0, 0, 0, 0)')
					.polygonStrokeColor(() => '#111');
					//.onGlobeReady(() => {
					//	globe.layers['polygons'].onPolygonClick(() => console.log('test'));
					//});


				const globeMaterial = globe.globeMaterial() as THREE.MeshPhongMaterial;
					globeMaterial.bumpScale = 10;

				new THREE.TextureLoader().load('./img/earth-water.png', texture => {
					globeMaterial.specularMap = texture;
					globeMaterial.specular = new THREE.Color('grey');
					globeMaterial.shininess = 15;
				});


				ref.current!.add(globe);
				console.log("Add Globe");
				console.log(globe);
			})
	}, []);

	console.log("Render globe");

	return (
		<group ref={ref}/>
	);
}

export default TGlobe;