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
import { changeWorldSettings } from '../store/World';
import TurnHistory from './TurnHistory';
import GameResult from './GameResult';

const Playfield: FC = () => {
  const ContextBridge = useContextBridge(ReactReduxContext);
  const dispatch = useAppDispatch();

  const username = useAppSelector(state => state.auth.username);
  const playersInGame = useAppSelector(state => state.game.players, shallowEqual);
  const game = useAppSelector(state => state.game, shallowEqual);

  useEffect(() => {
    const player = playersInGame?.find(playerInGame => playerInGame!.player.username === username);

    const intervalId = player && player.color === game.turn
      ? undefined
      : setInterval(() => {
          dispatch(fetchGame());
        }, 5000);

    return () => {
      clearInterval(intervalId);
		}
  }, [game.turn]);


  useEffect(() => {
    dispatch(changeWorldSettings({ show: false }));
  }, []);

  const [emptyCount, whiteCount, blackCount] = game.boards[game.step].reduce((state, value) =>
    value.reduce((state, value) => [
      state[Color.none] + Number(value == Color.none),
      state[Color.white] + Number(value == Color.white),
      state[Color.black] + Number(value == Color.black)
    ], [0, 0, 0])
      .map((count, index) => count + state[index])
    , [0, 0, 0]);

  return (
    <Box 
      component="div"
      sx={{
        position: 'absolute',
        top: 0,
        left: 0,
        right: 0,
        bottom: 0
      }}
    >
      <Box
        component="div"
        sx={{
          display: 'flex',
          flexDirection: 'row',
          width: '100%',
          height: '100%'
				}}
      >
        <Box
          component="div"
          sx={{
            position: 'relative',
            flexGrow: 1,
            height: '100%',
            overflow: 'hidden'
					}}
        >
          <Canvas
            frameloop="demand"
            style={{
              zIndex: 1
			      }}
          >
            <ContextBridge>
              <pointLight position={[10, 10, 10]} />
              <ambientLight color={'hotpink'} />
              <Board position={[0, 0, 0]} />
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
                    .find(playerInGame => playerInGame!.color === color)!
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
        </Box>
        <Box
          id="sidebar"
          component="div"
          sx={{
            flexGrow: 0,
            pt: '56px',
            height: '100%',
            backgroundColor: '#121212'
					}}
        >
          <TurnHistory/>
        </Box>
      </Box>
    </Box>
  );
}

export default Playfield;