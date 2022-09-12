import { Button, Snackbar } from '@mui/material';
import { FC, useState } from 'react';

const Challenge: FC = () => {
  const [open, setOpen] = useState(true);

  const handleDecline = () => {
    console.log('Declined');
    setOpen(false);
  }

  const handleAccept = () => {
    console.log('Accepted');
    setOpen(false);
	}

	return (
    <Snackbar
      open={open}
      autoHideDuration={6000}
      message="Someone has challenged you to a duel!"
      action={
       <>
          <Button
            size="small"
            color="success"
            onClick={handleAccept}
          >
            Accept
          </Button>
          <Button
            size="small"
            color="error"
            onClick={handleDecline}
          >
            Decline
        </Button>
       </>
      }
      sx={{ bottom: 90 }}
    />
		);
}

export default Challenge;