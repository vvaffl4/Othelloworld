import { Box } from '@mui/material';
import { OrbitControls, PerspectiveCamera, useContextBridge } from '@react-three/drei';
import { Canvas } from '@react-three/fiber';
import { FC, useEffect } from 'react';
import { ReactReduxContext, shallowEqual } from 'react-redux';
import * as THREE from 'three';
import Board from './threejs/Board';
import { Color } from '../model/PlayerInGame';
import { fetchGame } from '../store/Game';
import { useAppDispatch, useAppSelector } from '../store/Hooks';
import Score from './Score';
import Timeline from './Timeline';
import Camera from './threejs/Camera';

const Playfield: FC = () => {
  const ContextBridge = useContextBridge(ReactReduxContext);
  const dispatch = useAppDispatch();

  const username = useAppSelector(state => state.auth.username);
  const playersInGame = useAppSelector(state => state.game.players, shallowEqual);
  const game = useAppSelector(state => state.game, shallowEqual);

  useEffect(() => {
    const player = playersInGame?.find(playerInGame => playerInGame.player.username === username);

    const intervalId = player && player.color === game.turn
      ? undefined
      : setInterval(() => {
          dispatch(fetchGame());
        }, 5000);

    return () => {
      clearInterval(intervalId);
		}
  }, [game.turn]);

  const [emptyCount, whiteCount, blackCount] = game.board.reduce((state, value) =>
    value.reduce((state, value) => [
      state[Color.none] + Number(value == Color.none),
      state[Color.white] + Number(value == Color.white),
      state[Color.black] + Number(value == Color.black)
    ], [0, 0, 0])
      .map((count, index) => count + state[index])
    , [0, 0, 0]);

  return (
    <>
     <Canvas
        style={{
          position: 'absolute',
          top: 0,
          left: 0,
          right: 0,
          zIndex: 1
        }}
        frameloop="demand"
      >
        <ContextBridge>
          <pointLight position={[10, 10, 10]} />
          <ambientLight color={'hotpink'} />
          <Board position={[0, 0, 0]} />
          {/*<PerspectiveCamera*/}
          {/*  makeDefault*/}
          {/*  position={[0, 5, 5.5]}*/}
          {/*  rotation={new THREE.Euler(-45 * (Math.PI / 180), 0, 0)}*/}
          {/*/>*/}
          {/*<OrbitControls makeDefault minPolarAngle={0} maxPolarAngle={Math.PI / 1.75} />*/}
          <Camera />
        </ContextBridge>
      </Canvas>
      <Box
        component="div"
        sx={{
          position: 'absolute',
          top: 0,
          bottom: 0,
          left: 0,
          right: 0,
          pointerEvents: 'none'
        }}
      >
        {
          playersInGame && ([Color.white, Color.black])
            .map(color => ({
              color,
              playerInGame: playersInGame
                .find(playerInGame => playerInGame.color === color)!
            }))
            .map(({ color, playerInGame }) => (
              <Score
                side={playerInGame.player.username === username ? 'left' : 'right'}
                playerInGame={playerInGame}
                score={color === Color.black ? blackCount : whiteCount}
              />
            ))
        }
        <Timeline />
      </Box>
    </>
  );
}

export default Playfield;