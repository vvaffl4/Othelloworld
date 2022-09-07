import { Box, Container, Grid, Typography, useTheme } from '@mui/material';
import { FC } from 'react';
import { useAppSelector } from '../store/Hooks';

const Staging: FC = () => {
  const theme = useTheme();
  const game = useAppSelector(state => state.game);

	return (
    <Container
      component='div'
      sx={{
        position: 'absolute',
        top: 400,
        bottom: 0,
        left: 0,
        right: 0
      }}
    >
      <Box
        component="div"
        sx={{
          backgroundColor: 'black'
				}}
      >
        <Box
          component="span"
          sx={{
            position: 'absolute',
            top: '-50px',
            left: 0,
            display: 'block',
            width: '100px',
            height: '50px',
            overflow: 'hidden',
            ':after': {
              content: '""',
              width: '100px',
              height: '100px',
              borderRadius: '100px',
              background: 'rgba(0, 0, 0, 0)',
              position: 'absolute',
              top: '-100px',
              left: '-40px',
						}
          }}
        />
        <Grid container>
          <Grid item xs={6}> 
            <Typography
             variant="h3"
            >
            { game.players && game.players[0].username }
          </Typography>
          </Grid>
          <Grid item xs={6}>
            {game.players && game.players[0].username}
          </Grid>
        </Grid>
      </Box>
		</Container>
	);
}

export default Staging;