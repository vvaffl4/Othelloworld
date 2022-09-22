import { Box, Container, Grid, Typography, Paper } from '@mui/material';
import { FC, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { fetchGame } from '../store/Game';
import { useAppDispatch, useAppSelector } from '../store/Hooks';
import { changeWorldSettings } from '../store/World';
import PlayerCard from './PlayerCard';

const Staging: FC = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const game = useAppSelector(state => state.game);

  useEffect(() => {
    dispatch(changeWorldSettings({
      zoom: 3
    }))
    
    const fetchGameInterval = setInterval(() => {
      dispatch(fetchGame());
    }, 5000);

    return () => {
      clearInterval(fetchGameInterval);
		}
  }, []);

  useEffect(() => {
    if (game.players && game.players.length > 1) {
      navigate('/play');
		}
	}, [game])

	return (
    <Container
      component='div'
      sx={{
        position: 'absolute',
        top: 405,
        bottom: 0,
        left: 0,
        right: 0
      }}
    >
      <Paper
        elevation={5}
        sx={{
          position: 'relative',
          height: '100%',
          background: 'none',
				}}
      >
        <Grid
          container
          sx={{
            position: 'absolute',
            top: 0,
            left: 0,
            right: 0,
            display: 'flex'
          }}
        >
          <Grid
            item
            sx={{
              display: 'flex',
              flex: 1,
              height: '250px',
              backgroundColor: (theme) => theme.palette.background.paper
            }}
          >
            <Box
              component="span"
              sx={{
                mr: 'auto'
              }}
            />
            <PlayerCard player={game.players
              ? game.players[0].player
              : undefined} />
          </Grid>
          <Grid
            item
            flex={1}
          >
            <Box
              component="div"
              sx={{
                position: 'relative',
                marginTop: '100px',
                width: '500px',
                background: 'rgba(0, 0, 0, 0.5)',
                overflowY: 'show'
				      }}
            >
              <Box
                component="span"
                sx={{
                  position: 'absolute',
                  top: '-100px',
                  left: '50%',
                  marginLeft: '-250px',
                  display: 'block',
                  width: '500px',
                  height: '250px',
                  overflow: 'hidden',
                  ':after': {
                    content: '" "',
                    position: 'absolute',
                    top: '-400px',
                    left: '-150px',
                    width: '800px',
                    height: '800px',
                    backgroundColor: 'transparent',
                    border: (theme) => `150px solid ${theme.palette.background.paper}`,
                    borderRadius: '400px',
                    boxSizing: 'border-box'
						      }
                }}
              />
            </Box>
          </Grid>
          <Grid
            item
            sx={{
              display: 'flex',
              flex: 1,
              height: '250px',
              backgroundColor: (theme) => theme.palette.background.paper
            }}
          >
            <Box
              component="span"
              sx={{
                ml: 'auto',
                backgroundColor: (theme) => theme.palette.background.paper
              }}
            />
            <PlayerCard />
          </Grid>
        </Grid>
        <Grid
          container
          sx={{
            backgroundColor: (theme) => theme.palette.background.paper
          }}
        >
          <Grid item xs={6}> 
            <Typography
              variant="h3"
            >
          </Typography>
          </Grid>
          <Grid item xs={6}>
          </Grid>
        </Grid>
        <Box
          component="div"
          sx={{
            position: 'absolute',
            top: '250px',
            left: 0,
            right: 0,
            bottom: 0,
            backgroundColor: (theme) => theme.palette.background.paper
          }}
        >

         </Box>
      </Paper>
		</Container>
	);
}

export default Staging;