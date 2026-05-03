import {Grid} from '@mui/material'
import ActivityList from './ActivityList'
import ActivityDetails from '../details/ActivityDetails'
import ActivityForm from '../details/form/ActivityForm'
type Props = {
    activities: Activity[]
    selectActivity: (id: string) => void;
    cancelSelectActivity: () => void,
    selectedActivity?: Activity,
    openForm: (id: string) => void;
    closeForm: () => void;
    editMode: boolean
    submitForm: (activity: Activity) => void
    deleteActivity: (id: string) => void
}
export default function ActivityDashboard({activities,
   cancelSelectActivity, selectActivity, selectedActivity,
  openForm, editMode, closeForm, submitForm, deleteActivity}: Props) {
  return (
    <Grid container spacing={3}>
        <Grid size={7}>
            <ActivityList 
            activities={activities}
            selectActivity = {selectActivity}
            deleteActivity = {deleteActivity}/>
        </Grid>
        <Grid size={5}>
          {selectedActivity && !editMode && <ActivityDetails openForm = {openForm} activity={selectedActivity} cancelSelectActivity = {cancelSelectActivity}/>}
          {editMode && <ActivityForm closeForm = {closeForm} activity={selectedActivity} submitForm = {submitForm}/>}
        </Grid>
    </Grid>
  )
}
