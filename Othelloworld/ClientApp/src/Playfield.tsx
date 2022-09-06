import { Box, Typography, useTheme } from '@mui/material';
import { OrbitControls, PerspectiveCamera, useContextBridge } from '@react-three/drei';
import { Canvas } from '@react-three/fiber';
import { FC, useEffect } from 'react';
import { ReactReduxContext, shallowEqual } from 'react-redux';
import { useParams } from 'react-router-dom';
import * as THREE from 'three';
import Board from './Board';
import { Color } from './model/PlayerInGame';
import { fetchGame } from './store/Game';
import { useAppDispatch, useAppSelector } from './store/Hooks';

const Playfield: FC = () => {
  const theme = useTheme();
  const ContextBridge = useContextBridge(ReactReduxContext);
  const dispatch = useAppDispatch();

  const { gameToken } = useParams();
  const players = useAppSelector(state => state.game.players, shallowEqual);
  const board = useAppSelector(state => state.game.board, shallowEqual);

  useEffect(() => {
    if (gameToken) {
      dispatch(fetchGame(gameToken)); 
    }
  }, [gameToken]);

  const [emptyCount, whiteCount, blackCount] = board.reduce((state, value) =>
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
          right: 0
        }}
      >
        <ContextBridge>
          <pointLight position={[10, 10, 10]} />
          <ambientLight color={'hotpink'} />
          <Board position={[0, 0, 0]}/>
          <PerspectiveCamera 
            makeDefault 
            position={[0, 5, 5.5]}
            rotation={new THREE.Euler(-45 * (Math.PI / 180), 0, 0)}
          />
          <OrbitControls makeDefault minPolarAngle={0} maxPolarAngle={Math.PI / 1.75} />
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
        <Box
          component="div"
          sx={{
            position: 'absolute',
            bottom: 0,
            left: 0,
            width: '100px',
            height: '100px',
            zIndex: 3,
            background: theme.palette.background.paper,
            ':before': {
              content: '" "',
              position: 'absolute',
              top: 0,
              right: 0,
              width: '150px',
              height: '150px',
              transform: 'rotate(45deg)',
              transformOrigin: '100% 0%',
              backgroundColor: theme.palette.background.paper,
              zIndex: -1
						}
					}}
        >
          <Typography
            sx={{
              mt: '-120px',
              pl: '25px',
              fontSize: 72,
              fontWeight: 'bold',
              textShadow: `-7px 7px ${theme.palette.primary[theme.palette.mode]}`
						}}
          >
            {whiteCount}
          </Typography>
          <Typography>
            {players && players[0].color == Color.white
              ? 'White'
              : 'Black'
            }
          </Typography>
          <Typography>
            {players && players[0].username} Score
          </Typography>
        </Box>
        <Box
          component="div"
          sx={{
            position: 'absolute',
            top: '56px',
            right: 0,
            width: '100px',
            height: '100px',
            zIndex: 3,
            background: theme.palette.background.paper,
            ':before': {
              content: '" "',
              position: 'absolute',
              bottom: 0,
              keft: 0,
              width: '150px',
              height: '150px',
              transform: 'rotate(45deg)',
              transformOrigin: '0% 100%',
              backgroundColor: theme.palette.background.paper,
              zIndex: -1
            }
          }}
        >
          <Typography
            variant="h5"
          >
            {players && players[1].username} Score
          </Typography>
          <Typography>
            {players && players[1].color == Color.white
              ? 'White'
              : 'Black'
						}
          </Typography>
          <Typography
            sx={{
              mt: '40px',
              pr: '25px',
              fontSize: 72,
              fontWeight: 'bold',
              textShadow: `7px -7px ${theme.palette.primary[theme.palette.mode]}`
            }}
          >
            {blackCount}
          </Typography>
        </Box>
      </Box>
    </>
  );
}

export default Playfield;