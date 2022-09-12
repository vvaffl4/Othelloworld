import { Box, Button, Card, CardActions, CardContent, Typography } from '@mui/material';
import { FC } from 'react';
import Player from '../model/Player';

const PlayerCard: FC<{ player?: Player }> = ({ player }) => {
  const username = player && player.username.charAt(0).toUpperCase() + player.username.slice(1);

	return (
    <Box
      component="div"
      sx={{ width: '100%' }}
    >
      <Box
        component="div"
        sx={{
          mt: '-60px',
          mb: '-60px',
          width: '100%',
          height: '120px',
          textAlign: 'center'
        }}
      >
        <Box
          component="span"
          className="fi fi-nl"
          style={{
            display: 'inline-block',
            width: '160px',
            height: '120px',
            fontSize: 36,
            borderRadius: '10px',
            overflow: 'hidden',
            boxShadow: '0px 2px 4px -1px #0003,0px 4px 5px 0px #00000024,0px 1px 10px 0px #0000001f'
          }}
        />
      </Box>
      <Card
        variant="outlined"
        sx={{ m: 4, pt: 4 }}
      >
        <CardContent>
          <Typography variant="h5" component="div">
            { username }
          </Typography>
          <Typography sx={{ mb: 1.5 }} color="text.secondary">
            The Netherlands
          </Typography>
          <Typography variant="body2">
            well meaning and kindly.
            <br />
            {'"a benevolent smile"'}
          </Typography>
        </CardContent>
        <CardActions>
          <Button size="small">Learn More</Button>
        </CardActions>
			</Card>
		</Box>
	);
}

export default PlayerCard;