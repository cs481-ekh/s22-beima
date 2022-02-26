import {useDropzone} from 'react-dropzone';
import styles from './ImageFileUpload.module.css';

const ImageFileUpload = ({type, details, multiple, acceptTypes}) => {
  const {acceptedFiles, getRootProps, getInputProps} = useDropzone({
    accept: acceptTypes,
    multiple: multiple,
    noDrag: true
  });

  const fileList = acceptedFiles.map(file => (
      <li key={file.name}>
        {file.name}
      </li>
  ));

  return (
    <div>
      <section className={styles.fileupload}>
        <div {...getRootProps({className: 'dropzone'})}>
          <input {...getInputProps()} />
          <div>Click to select {type} {details}</div>
        </div>
      </section>
      <aside>
        <h6>{type} Uploaded:</h6>
        <ul>{fileList}</ul>
      </aside>
    </div>
  );
}

export default ImageFileUpload