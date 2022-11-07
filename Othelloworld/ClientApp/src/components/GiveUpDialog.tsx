import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from "@mui/material";
import { FC } from "react";
import { giveUp } from "../store/Game";
import { useAppDispatch } from "../store/Hooks";

interface GiveUpDialogProps {
  show: boolean;
  onClose: () => void;
}

const GiveUpDialog: FC<GiveUpDialogProps> = ({ show, onClose }) => {
  const dispatch = useAppDispatch();

  const handleGiveUp = () => {
    dispatch(giveUp());
	}

	return (
    <Dialog
      open={show}
      onClose={onClose}
      aria-labelledby="alert-dialog-title"
      aria-describedby="alert-dialog-description"
    >
      <DialogTitle id="alert-dialog-title">
        Give up?
      </DialogTitle>
      <DialogContent>
        <DialogContentText id="alert-dialog-description">
          Do you really want to give up this match?
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button
          variant="contained"
          color="error"
          onClick={onClose}
          autoFocus
        >
          No
        </Button>
        <Button
          variant="text"
          onClick={handleGiveUp}
        >
          Yes
        </Button>
      </DialogActions>
    </Dialog>
	);
}

export default GiveUpDialog;